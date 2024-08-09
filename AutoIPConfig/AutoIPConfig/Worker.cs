using AutoIPConfig.Helper;
using AutoIPConfig.Model.NetConfig;
using AutoIPConfig.Model;
using AutoIPConfig.Strategy;
using Newtonsoft.Json;

namespace AutoIPConfig
{
    public class Worker : BackgroundService
    {
        /// <summary>
        /// 运行配置
        /// </summary>
        private Config RunningConfig { get; set; }
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    DoWork();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during auto config:" + ex.Message);
                }

                await Task.Delay(1000 * 5, stoppingToken);
            }
        }

        public void DoWork()
        {
            //读取配置文件
            var config = AppSettingsHelper.GetObject<Config>("AutoConfig");

            if (config == null)
            {
                return;
            }

            RunningConfig = config;

            //尝试从USB获取文件
            var usbFile = LoadConfigInUSB();

            if (usbFile == null)
            {
                LogHelperEx.Debug("No USB device found.");
                return;
            }

            LogHelperEx.Debug("config file found on USB device." + JsonConvert.SerializeObject(usbFile));


            ShelfAutoConfig(usbFile);
        }

        /// <summary>
        /// 配置货架信息
        /// </summary>
        /// <param name="config"></param>
        private void ShelfAutoConfig(NetworkConfig config)
        {
            AutoConfigStrategy strategy = null;
            switch (config.Mode)
            {
                case Model.Constant.NetworkModeEnum.WIFI:
                    if (config is WifiConfig wifiConfig)
                    {
                        strategy = new WifiAutoConfigStrategy(wifiConfig);
                    }
                    else
                    {
                        LogHelperEx.Debug($"config type error, mode:{config.Mode}");
                        throw new Exception($"config type error, mode:{config.Mode}");
                    }
                    break;
                case Model.Constant.NetworkModeEnum.ETHERNET:
                    break;
                default:
                    break;
            }

            if (strategy == null)
            {
                LogHelperEx.Debug($"config strategy not found, mode:{config.Mode}");
                throw new Exception($"config strategy not found, mode:{config.Mode}");
            }
            strategy.AutoConfig();
        }

        /// <summary>
        /// 从USB设备中加载配置文件
        /// </summary>
        /// <returns></returns>
        private NetworkConfig LoadConfigInUSB()
        {
            //获取所有USB设备
            var usbInfo = USBHelper.GetRemovableDiskDevice();
            if (usbInfo == null || usbInfo.Count == 0)
            {
                return null;
            }

            NetworkConfig result = null;


            foreach (var item in usbInfo)
            {
                bool isAutoMount = false;
                //如果设备没挂载则尝试挂载
                if (string.IsNullOrEmpty(item.mountpoint))
                {
                    //TODO:挂载设备
                    USBHelper.Mount(item.name);

                    //保存挂载点
                    item.mountpoint = item.name;

                    isAutoMount = true;
                }

                //遍历所有USB设备，找到符合条件的设备

                //尝试解析配置文件
                var filePath = $"{USBHelper.MountDir}/{item.mountpoint}/{RunningConfig.FileName}";
                LogHelperEx.Debug("filePath:" + filePath);

                var fileText = string.Empty;
                var configFile = new FileInfo(filePath);
                if (configFile.Exists)
                {
                    //TODO:读取配置文件内容
                    //return new WifiConfig();
                    fileText = File.ReadAllText(configFile.FullName);
                    LogHelperEx.Debug("fileInfo:" + fileText);
                }

                //释放主动挂载的设备
                if (isAutoMount)
                {
                    //TODO:释放设备
                    USBHelper.Umount(item.name);
                }

                if (!string.IsNullOrEmpty(fileText))
                {
                    try
                    {
                        result = ParseConfig(fileText);
                    }
                    catch (Exception e)
                    {
                        LogHelperEx.Debug("can not parse file content:" + e.Message);
                    }
                }
            }


            return result;
        }

        private static NetworkConfig ParseConfig(string fileText)
        {
            //TODO:解析配置文件内容，并转换为NetworkConfig对象
            NetworkConfig result = JsonConvert.DeserializeObject<NetworkConfig>(fileText);
            switch (result.Mode)
            {
                case Model.Constant.NetworkModeEnum.WIFI:
                    var wifiConfig = JsonConvert.DeserializeObject<WifiConfig>(fileText);
                    result = wifiConfig;
                    break;
                case Model.Constant.NetworkModeEnum.ETHERNET:
                    var ethernetConfig = JsonConvert.DeserializeObject<EthernetConfig>(fileText);
                    result = ethernetConfig;
                    break;
                default:
                    LogHelperEx.Debug($"config mode error,mode:{result.Mode}");
                    throw new Exception($"config mode error,mode:{result.Mode}");
                    break;
            }

            return result;
        }
    }
}
