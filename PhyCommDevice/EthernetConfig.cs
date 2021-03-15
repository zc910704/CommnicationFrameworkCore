using CommDeviceCore.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore.PhysicalCommDevice
{
    class EthernetConfig : IDeviceConfig
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}
