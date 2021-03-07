using CommDeviceCore.Common.Interface;
using CommDeviceCore.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommDeviceCore.Common
{
    public interface IPhyCommDevice: IDevice,IDisposable
    {
        public IDeviceConfig DeviceConfig { get; set; }
        ITransportLayerProtocol TransportLayerProtocol { get; }

        public IApplicationLayerProtocol ApplicationLayerProtocol { get; }

        public Task<byte[]> Send(byte[] package, CancellationToken cancellationToken);

        public Task<ILayPackageSendResult> Send(IDeviceCommand command, CancellationToken cancellationToken);
    }
}
