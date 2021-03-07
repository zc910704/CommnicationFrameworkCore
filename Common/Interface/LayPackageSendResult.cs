using System;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore.Common.Interface
{
    public class LayPackageSendResult : ILayPackageSendResult
    {
        public IDeviceCommand Command
        {
            get; set;
        }
        public bool Status
        {
            get; set;
        }

        public LayPackageSendResult(IDeviceCommand Command, bool status)
        {
            this.Command = Command;
            this.Status = status;
        }
    }
}
