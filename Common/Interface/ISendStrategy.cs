using CommDeviceCore.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommDeviceCore.Common
{
    public interface ISendStrategy
    {
        public IPhyCommDevice PhyCommDevice { get; set; }

        public Task<ILayPackageSendResult> Send(IDeviceCommand cmd);

        public Task<ILayPackageSendResult> Send(IList<IDeviceCommand> cmd);
    }
}
