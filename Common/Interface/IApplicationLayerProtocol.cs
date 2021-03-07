using System;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore.Common.Interface
{
    public interface IApplicationLayerProtocol:IProtocol
    {
        public byte[] Pack(IDeviceCommand Cmd);

        public IDeviceCommand Unpack(byte[] payload, IDeviceCommand command);
    }
}
