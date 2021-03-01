using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommDeviceCore.PhysicalCommDevice
{
    public interface IPhyCommDevice: IDevice,IDisposable
    {
        public IDeviceConfig DeviceConfig { get; set; }

        public Task<byte[]> Send(byte[] package);
    }
}
