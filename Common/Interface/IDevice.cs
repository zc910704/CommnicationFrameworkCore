using System;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore
{
    public interface IDevice
    {
        public bool IsOpen { get; set; }
    }
}
