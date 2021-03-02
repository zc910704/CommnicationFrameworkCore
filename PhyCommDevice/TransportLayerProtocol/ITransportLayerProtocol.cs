using System;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore.Protocol
{
    public interface ITransportLayerProtocol : IProtocol
    {
        public int MiniumResponseLength { get => 8; }

        public int GetLengthFromHeader(byte[] buff);
    }
}
