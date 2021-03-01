using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace CommDeviceCore.PhysicalCommDevice
{
    public class SerialDevice : IPhyCommDevice
    {
        public IDeviceConfig DeviceConfig { get; set; }
        public bool IsOpen { get => _SerialPort.IsOpen; }

        private SerialPort _SerialPort;

        private object _LockObject = new object();
        private bool disposedValue;

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
                }
                else throw new InvalidCastException($"InvalidCast {nameof(DeviceConfig)} to type SerialConfig");
            }
        }

        public Task<byte[]> Send(byte[] content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            lock (_LockObject)
            {
                return Task.Run(
                    ()=>
                    {
                        if (IsOpen == false) this.Open();
                        _SerialPort.BaseStream.Write(content, 0, content.Length);
                        _SerialPort.BaseStream.ReadAsync();
                    }                    
                    );
                
            }
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~SerialDevice()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        } 
        #endregion
    }
}
