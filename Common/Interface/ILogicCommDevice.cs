using CommDeviceCore.Common;
using CommDeviceCore.Common.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CommDeviceCore.LogicalCommDevice
{
    public interface ILogicCommDevice: IDevice
    {
        /// <summary>
        /// 策略模式
        /// </summary>
        public ISendStrategy SendStrategy { get; }

        public IPhyCommDevice PhyCommDevice { get; }

        public Task<ILayPackageSendResult> Send(IDeviceCommand cmd, CancellationToken cancellationToken) => SendStrategy.Send(cmd,cancellationToken);

        public Task<ILayPackageSendResult> Send(IList<IDeviceCommand> cmds, CancellationToken cancellationToken) => SendStrategy.Send(cmds, cancellationToken);
    }
}
