using CommDeviceCore.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommDeviceCore.LogicalCommDevice
{
    public class DirectSendStrategy : ISendStrategy
    {
        public IPhyCommDevice PhyCommDevice { get ; }

        public DirectSendStrategy(IPhyCommDevice phyCommDevice)
        {
            this.PhyCommDevice = phyCommDevice;
        }

        public async Task<ILayPackageSendResult> Send(IDeviceCommand cmd, CancellationToken cancellationToken)
        {
            return await Task.Run(() => PhyCommDevice.Send(cmd, cancellationToken));
        }

        public async Task<ILayPackageSendResult> Send(IList<IDeviceCommand> cmds, CancellationToken cancellationToken)
        {
            foreach (var cmd in cmds)
            { 
                return await Task.Run(() => PhyCommDevice.Send(cmd, cancellationToken));
            }
            return null;
        }
    }
}
