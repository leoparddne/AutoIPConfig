using AutoIPConfig.Model;
using AutoIPConfig.Model.Constant;
using Newtonsoft.Json;

namespace AutoIPConfig.Helper
{
    public class USBHelper
    {
        /// <summary>
        /// /mnt
        /// </summary>
        public static string MountDir = "/mnt";

        /// <summary>
        /// /dev
        /// </summary>
        public static string DevDir = "/dev";


        /// <summary>
        /// 获取所有USB设备
        /// </summary>
        /// <returns></returns>
        public static DiskBlock GetUSBList()
        {
            var result = GetUSBDeviceInfo();

            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            try
            {
                var deviceInfo = JsonConvert.DeserializeObject<DiskBlock>(result);

                return deviceInfo;
            }
            catch (System.Exception e)
            {
                LogHelperEx.Debug(e.Message);
                throw;
            }


            return null;
        }

        /// <summary>
        /// 获取所有可移动的设备,如果有子设备，则返回子设备
        /// </summary>
        /// <returns></returns>
        public static List<Blockdevice> GetRemovableDiskDevice()
        {
            List<Blockdevice> allUSB = new();
            var removebleDevice = GetRemovebleDiskDevice();
            if (removebleDevice == null || removebleDevice.Count == 0)
            {
                return allUSB;
            }

            LogHelperEx.Debug("GetRemovebleDiskDevice:" + JsonConvert.SerializeObject(removebleDevice));

            //获取所有可移动的设备
            foreach (var item in removebleDevice)
            {
                if (item.children != null && item.children.Length > 0)
                {
                    foreach (var child in item.children)
                    {
                        allUSB.Add(child);
                    }
                }
                else
                {
                    if (item.rm == true)
                    {
                        allUSB.Add(item);
                    }
                }
            }

            return allUSB;
        }

        /// <summary>
        /// 获取所有USB设备(存储设备 type:disk )
        /// </summary>
        /// <returns></returns>
        private static List<Blockdevice> GetRemovebleDiskDevice()
        {
            //获取所有USB设备
            var usbInfo = USBHelper.GetUSBList();
            if (usbInfo == null || usbInfo.blockdevices == null || usbInfo.blockdevices.Length == 0)
            {
                return null;
            }

            LogHelperEx.Debug("devicelist:" + JsonConvert.SerializeObject(usbInfo));

            var result = usbInfo.blockdevices.Where(f => f.rm == true && f.type == "disk").ToList();
            return result;
        }

        private static string GetUSBDeviceInfo()
        {
            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);
            command.AddParameter(" -c \"lsblk -J\" ");
            var result = command.Exec(true);
            LogHelperEx.Debug(result);

            return result;
        }


        /// <summary>
        /// 挂载USB设备
        /// </summary>
        /// <returns></returns>
        public static List<string> MountUSB()
        {
            List<string> usbList = new List<string>();
            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);


            return usbList;
        }


        /// <summary>
        /// 卸载USB设备
        /// </summary>
        /// <returns></returns>
        public static List<string> UmountUSB()
        {
            List<string> usbList = new List<string>();
            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);


            return usbList;
        }

        public static void Mount(string name)
        {
            //目标挂载目录
            var mountFinalPath = MountDir + "/" + name;
            var sourceDev = DevDir + "/" + name;

            if (!Directory.Exists(mountFinalPath))
            {
                Directory.CreateDirectory(mountFinalPath);
            }

            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);

            command.AddParameter($" -c \"sudo mount {sourceDev} {mountFinalPath}\" ");
            command.Exec(true);
        }




        public static void Umount(string name)
        {
            //目标挂载目录
            var mountFinalPath = MountDir + "/" + name;

            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);

            command.AddParameter($" -c \"sudo umount {mountFinalPath}\" ");
            command.Exec(true);
        }
    }
}
