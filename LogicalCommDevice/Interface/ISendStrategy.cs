using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommDeviceCore
{
    public interface ISendStrategy
    {
        public Task<ILayPackageSendResult> Send(IDeviceCommand cmd);
    }
}
