using CommDeviceCore.LogicalCommDevice;
using CommDeviceCore.PrivateProtocol;
using CommDeviceCore.PhysicalCommDevice;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using CommDeviceCore.Common.Utils;
using System.Threading;

namespace CommDeviceCore.LogicalCommDevice.Tests
{
    public class Test
    {
        [Fact()]
        public async void OpenTest()
        {
            WayeeTransportProtocol transProtocol = new ();
            WayeeApplicationProtocol appProtocol = WayeeApplicationProtocol.Convert(@"D:\Device.xml");
            SerialDevice device = new(transProtocol, appProtocol);
            device.DeviceConfig = new SerialConfig() { PortName = "COM9", BaudRate = 9600 };
            device.Open();
            DirectSendStrategy directSend = new(device);
            ILogicCommDevice logicalCommDevice = new LogicalCommDevice(device, directSend);
            DateTime start = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                var cmd = appProtocol.Commands[0];
                CancellationTokenSource cts = new CancellationTokenSource();
                var res = await logicalCommDevice.Send(cmd, cts.Token);
                Assert.True(res.Status);
            }
            DateTime stop = DateTime.Now;
            var span = stop - start;
            var all = span.TotalSeconds;
        }
    }
}

namespace CommDeviceCore.PrivateProtocol.Tests
{
    public class Test
    {
        [Fact()]
        public void CRC16Test()
        {
            byte[] test = new byte[] { 0x11, 0x22, 0x33, 0x44 };
            var crc2 = MathUtils.CalculateCRC16(test);
            Assert.True(true, "This test needs an implementation");
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
