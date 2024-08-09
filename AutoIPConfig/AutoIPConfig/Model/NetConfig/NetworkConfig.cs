using AutoIPConfig.Model.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoIPConfig.Model.NetConfig
{
    /// <summary>
    /// 网络配置
    /// </summary>
    public class NetworkConfig
    {
        /// <summary>
        /// 运行模式 0:无线 1:有线
        /// </summary>
        public NetworkModeEnum Mode { get; set; }

        /// <summary>
        /// 是否启用DHCP
        /// </summary>
        public bool ISDHCP { get; set; }

        public string IP { get; set; }

        public string SubnetMask { get; set; }

        public string Gateway { get; set; }

        public string DNS { get; set; }
    }
}
