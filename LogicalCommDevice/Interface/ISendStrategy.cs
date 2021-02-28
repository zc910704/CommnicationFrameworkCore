using CommDeviceCore.PhysicalCommDevice;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommDeviceCore
{
    public interface ISendStrategy
    {
        public IPhyCommDevice PhyCommDevice { get; set; }

        public Task<ILayPackageSendResult> Send(IDeviceCommand cmd);

        public Task<ILayPackageSendResult> Send(IList<IDeviceCommand> cmd);
    }
}
