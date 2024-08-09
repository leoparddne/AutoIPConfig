using AutoIPConfig.Helper;
using AutoIPConfig.Model.NetConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoIPConfig.Strategy
{
    public class WifiAutoConfigStrategy : AutoConfigStrategy
    {
        public new WifiConfig Config { get; }

        public WifiAutoConfigStrategy(WifiConfig config) : base(config)
        {
            this.Config = config;
        }

        public override void AutoConfig()
        {
            //TODO -检测是否启用WIFI
            //TODO -如果未启用WIFI，则启用WIFI
            //nmcli connection up wlan0
            //WifiHelper.EnableWifi();

            //解析wifi对应的UUID
            //nmcli con show


            //TODO - 检测所有可用的WIFI
            //nmcli dev wifi list


            //校验是否连接过WiFi
            //如果当前正连接目标WIFI则退出
            var currentWifi = WifiHelper.GetCurrentWifiSSID();
            LogHelperEx.Debug($"Current Wifi: {currentWifi}");
            if (currentWifi == Config.SSID)
            {
                LogHelperEx.Debug($"Already connected to target Wifi: {Config.SSID}");
                return;
            }

            //连接对应的SSID
            //sudo nmcli dev wifi connect network-ssid password "network-password"
            //nmcli con down ssid/uuid
            WifiHelper.ConnectToWifi(Config.SSID, Config.Password);

            //如果是动态分配IP，则退出
            if (Config.ISDHCP)
            {
                return;
            }

            //设置IP、子网掩码、网关、DNS
            WifiHelper.SetIP(Config.SSID, Config.IP, Config.SubnetMask, Config.Gateway, Config.DNS);

            //重新加载配置
            WifiHelper.ReloadConnection();

            //断开连接
            WifiHelper.DisconnectFromWifi(Config.SSID);

            //重新连接wifi
            WifiHelper.ConnectToWifi(Config.SSID);
        }
    }
}
