using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommDeviceCore.PhysicalCommDevice
{
    public interface IPhyCommDevice: IDevice,IDisposable
    {
        public IDeviceConfig DeviceConfig { get; set; }
        ITransportLayerProtocol TransportLayerProtocol { get; }

        public Task<byte[]> Send(byte[] package, CancellationToken cancellationToken);
    }
}
