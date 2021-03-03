using System;
using System.Collections.Generic;
using System.Text;
using CommDeviceCore.Common;

namespace CommDeviceCore.PhysicalCommDevice
{
    public class SerialConfig: IDeviceConfig
    {
        public string PortName { set; get; }

        public int BaudRate { set; get; }
    }
}
