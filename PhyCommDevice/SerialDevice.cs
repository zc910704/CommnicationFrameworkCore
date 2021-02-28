using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace CommDeviceCore.PhysicalCommDevice
{
    public class SerialDevice : IPhyCommDevice
    {
        public bool IsOpen { get; set; } = false;
        public IDeviceConfig DeviceConfig { get; set; }

        private SerialPort _SerialPort;

        private object _LockObject = new object();

        public void Close()
        {
            if (_SerialPort.IsOpen == true)
            {
                _SerialPort.Close();
            }
        }

        public void Open()
        {
            if (_SerialPort.IsOpen == false)
            {
                if (DeviceConfig == null) throw new NullReferenceException(nameof(DeviceConfig));
                if (DeviceConfig is SerialConfig cfg)
                {
                    _SerialPort.BaudRate = cfg.BaudRate;
                    _SerialPort.PortName = cfg.PortName;
                    _SerialPort.Open();
                    IsOpen = true;
                }
                else throw new InvalidCastException($"InvalidCast {nameof(DeviceConfig)} to type SerialConfig");
            }
        }

        public Task<ILayPackageSendResult> Send(ILayPackage package)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
            lock (_LockObject)
            {
                if (IsOpen == false)
                { 
                
                }
            }
        }
    }
}
