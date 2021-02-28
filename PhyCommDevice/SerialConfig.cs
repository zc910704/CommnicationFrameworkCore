using System;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore.PhysicalCommDevice
{
    public class SerialConfig:IDeviceConfig
    {
        public string PortName { set; get; }

        public int BaudRate { set; get; }
    }
}
