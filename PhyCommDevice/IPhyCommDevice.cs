using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommDeviceCore.PhysicalCommDevice
{
    public interface IPhyCommDevice: IDevice
    {
        public IDeviceConfig DeviceConfig { get; set; }

        public Task<ILayPackageSendResult> Send(ILayerPackage package);
    }
}
