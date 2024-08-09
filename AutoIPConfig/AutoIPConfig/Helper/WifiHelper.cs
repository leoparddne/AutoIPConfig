using AutoIPConfig.Model.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoIPConfig.Helper
{
    public class WifiHelper
    {
        public static void ConnectToWifi(string ssid, string password)
        {
            //sudo nmcli dev wifi connect network-ssid password "network-password"
            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);

            command.AddParameter($" -c \"nmcli dev wifi connect {ssid} password \"{password}\"\" ");
            command.Exec(true);
        }

        public static void DisconnectFromWifi(string ssid)
        {
            //nmcli con down ssid/uuid
            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);

            command.AddParameter($" -c \"nmcli con down {ssid} \" ");
            command.Exec(true);
        }



        public static void ConnectToWifi(string ssid)
        {
            //nmcli con up ssid/ uuid
            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);

            command.AddParameter($" -c \"nmcli con up {ssid} \" ");
            command.Exec(true);
        }


        public static void EnableWifi(string ssid)
        {
            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);

            //nmcli con up ssid/uuid
            command.AddParameter($" -c \"nmcli connection up {ssid}\" ");
            command.Exec(true);
        }


        public static string GetCurrentWifiSSID()
        {
            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);

            command.AddParameter($" -c \"nmcli connection show --active\" ");
            var execResult = command.Exec(true);

            LogHelperEx.Debug($"GetCurrentWifiSSID: {execResult}");

            //TEMP: Save output to file for debug
            File.WriteAllText("ssid.txt", execResult);


            if (string.IsNullOrEmpty(execResult))
            {
                return string.Empty;
            }

            var splitLineResult = execResult.Split('\n');

            if (splitLineResult.Length <= 1)
            {
                return string.Empty;
            }

            var currentSSIDText = splitLineResult[1].Trim();

            var ssidSplit = currentSSIDText.Split(' ');

            if (ssidSplit.Length <= 1)
            {
                return string.Empty;
            }

            return ssidSplit[0].Trim();
        }


        public static void SetIP(string ssid, string ip, string subnetMask, string gateway, string dns)
        {
            //nmcli connection modify ens38_1 ipv4.addresses 172.16.93.140/24 ipv4.geteway 172.16.93.3 ipv4.dns 172.16.93.2
            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);

            command.AddParameter($" -c \"nmcli connection modify {ssid} ipv4.addresses {ip}/{subnetMask} ipv4.gateway {gateway} ipv4.dns {dns}\" ");
            command.Exec(true);
        }

        /// <summary>
        /// 重置网卡连接
        /// </summary>
        /// <param name="sSID"></param>
        public static void ReloadConnection()
        {
            //nmcli connection reload
            ProcessCommandBase command = new ProcessCommandBase(CommonConstant.ShellPath);

            command.AddParameter($" -c \"nmcli connection reload\" ");
            command.Exec(true);
        }
    }
}
