using System;
using System.Collections.Generic;
using System.Text;

namespace CommDeviceCore.Common
{
    public interface ITransportLayerProtocol : IProtocol
    {
        public int MiniumResponseLength { get => 8; }

        public int? GetLengthFromHeader(byte[] buff);

        public byte[] Pack(byte[] payload);

        public byte[] Unpack(byte[] dataBuf);
    }
}
