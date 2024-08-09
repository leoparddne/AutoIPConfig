using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoIPConfig.Model
{
    public class DiskBlock
    {
        public Blockdevice[] blockdevices { get; set; }
    }

    public class Blockdevice
    {
        public string name { get; set; }
        public string majmin { get; set; }
        public bool rm { get; set; }
        public string size { get; set; }
        public bool ro { get; set; }
        public string type { get; set; }
        public string mountpoint { get; set; }
        public Blockdevice[] children { get; set; }
    }

}
