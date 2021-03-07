using System;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore.Common
{
    public interface ILayPackageSendResult
    {
        public IDeviceCommand Command { get; set; }

        public bool Status { get; set; }
    }
}
