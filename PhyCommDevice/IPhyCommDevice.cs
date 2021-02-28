using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommDeviceCore.PhysicalCommDevice
{
    public interface IPhyCommDevice: IDevice
    {
        public IDeviceConfig DeviceConfig { get; set; }

        public void Open();

        public void Close();

        public Task<ILayPackageSendResult> Send(ILayPackage package);
    }
}
