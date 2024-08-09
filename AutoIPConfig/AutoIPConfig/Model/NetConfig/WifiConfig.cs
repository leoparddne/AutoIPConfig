using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoIPConfig.Model.NetConfig
{
    /// <summary>
    /// 无线网络配置
    /// </summary>
    public class WifiConfig : NetworkConfig
    {
        public string SSID { get; set; }

        public string Password { get; set; }
    }
}
