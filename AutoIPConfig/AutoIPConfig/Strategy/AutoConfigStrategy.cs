using AutoIPConfig.Model.NetConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoIPConfig.Strategy
{
    public abstract class AutoConfigStrategy
    {
        public NetworkConfig Config { get; }

        public AutoConfigStrategy(NetworkConfig config)
        {
            Config = config;
        }

        /// <summary>
        /// 自动配置
        /// </summary>
        public abstract void AutoConfig();
    }
}
