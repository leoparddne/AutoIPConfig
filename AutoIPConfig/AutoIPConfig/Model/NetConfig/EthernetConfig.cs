using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoIPConfig.Model.NetConfig
{

    /// <summary>
    /// 以太网配置
    /// </summary>
    public class EthernetConfig : NetworkConfig
    {
        public string SSID { get; set; }

        public string Password { get; set; }
    }
}
