using CommDeviceCore.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CommDeviceCore.Common
{
    public interface ISendStrategy
    {
        public IPhyCommDevice PhyCommDevice { get; }

        public Task<ILayPackageSendResult> Send(IDeviceCommand cmd, CancellationToken cancellationToken);

        public Task<ILayPackageSendResult> Send(IList<IDeviceCommand> cmd, CancellationToken cancellationToken);

        
    }
}
