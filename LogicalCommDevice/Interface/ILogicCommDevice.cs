using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommDeviceCore
{
    public interface ILogicCommDevice
    {
        /// <summary>
        /// 策略模式
        /// </summary>
        public ISendStrategy SendStrategy { get; set; }

        public bool Open();

        public bool Close();

        public Task<ILayPackageSendResult> Send(IDeviceCommand cmd);

        public Task<ILayPackageSendResult> Send(IList<IDeviceCommand> cmd);
    }
}
