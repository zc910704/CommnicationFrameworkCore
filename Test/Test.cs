using CommDeviceCore.PrivateProtocol;
using CommDeviceCore.PhysicalCommDevice;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using CommDeviceCore.Common.Utils;

namespace CommDeviceCore.PrivateProtocol.Tests
{
    public class Test
    {
        [Fact()]
        public void CRC16Test()
        {
            byte[] test = new byte[] {0x11, 0x22, 0x33, 0x44 };
            var crc1 = MathHelper.CRC16(test);
            var crc2 = MathUtils.CalculateCRC16(test);
            Assert.True(crc1 == crc2, "This test needs an implementation");
        }
    }
}

namespace CommDeviceCore.PhysicalCommDevice.Tests
{
    public class Test
    {
        [Fact()]
        public void OpenTest()
        {
            Assert.True(true, "This test needs an implementation");
        }
    }
}
