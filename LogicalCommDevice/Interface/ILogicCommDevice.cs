using CommDeviceCore.PhysicalCommDevice;
using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommDeviceCore.LogicalCommDevice
{
    public interface ILogicCommDevice: IDevice
    {
        /// <summary>
        /// 策略模式
        /// </summary>
        public ISendStrategy SendStrategy { get; set; }

        public IPhyCommDevice PhyCommDevice { get; set; }

        public void Open();

        public void Close();

        public Task<ILayPackageSendResult> Send(IDeviceCommand cmd) => SendStrategy.Send(cmd);

        public Task<ILayPackageSendResult> Send(IList<IDeviceCommand> cmds) => SendStrategy.Send(cmds);
    }
}
