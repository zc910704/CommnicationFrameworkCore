using System;
using System.Collections.Generic;
using System.Text;
using System.Data.HashFunction.CRC;

namespace CommDeviceCore.Common.Utils
{
    public static class MathUtils
    {
        public static readonly ICRC crc = CRCFactory.Instance.Create(CRCConfig.XMODEM);

        public static UInt16 CalculateCRC16(byte[] data)
        {
            var value = crc.ComputeHash(data);
            var hash = BitConverter.ToUInt16(value.Hash,0);
            return hash;
        }
    }
}
