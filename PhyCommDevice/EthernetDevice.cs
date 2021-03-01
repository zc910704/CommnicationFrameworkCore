using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommDeviceCore.PhysicalCommDevice
{
    public class EthernetDevice : IPhyCommDevice
    {
        public bool IsOpen { get => throw new NotImplementedException(); }
        public IDeviceConfig DeviceConfig { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public Task<ILayPackageSendResult> Send(ILayerPackage package)
        {
            throw new NotImplementedException();
        }
    }
}
