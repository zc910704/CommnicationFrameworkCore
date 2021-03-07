using CommDeviceCore.Common;
using CommDeviceCore.Common.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore.LogicalCommDevice
{
    public class LogicalCommDevice : ILogicCommDevice
    {
        public ISendStrategy SendStrategy { get ; }

        public IPhyCommDevice PhyCommDevice { get; }

        public bool IsOpen => PhyCommDevice.IsOpen;

        public LogicalCommDevice(IPhyCommDevice phyCommDevice, ISendStrategy strategy)
        {
            this.PhyCommDevice = phyCommDevice;
            this.SendStrategy = strategy;
        }
        public void Open()
        {
            PhyCommDevice.Open();
        }

        public void Close()
        {
            PhyCommDevice.Close();
        }
    }
}
