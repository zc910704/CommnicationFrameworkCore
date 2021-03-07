using CommDeviceCore.Common;
using CommDeviceCore.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommDeviceCore.PhysicalCommDevice
{
    public class SerialDevice : IPhyCommDevice
    {
        public IDeviceConfig DeviceConfig { get; set; }
        public bool IsOpen { get => _SerialPort.IsOpen; }

        public ITransportLayerProtocol TransportLayerProtocol { get; }

        public IApplicationLayerProtocol ApplicationLayerProtocol { get; }

        private SerialPort _SerialPort;

        private object _LockObject = new object();

        private bool disposedValue;

        public SerialDevice(ITransportLayerProtocol transportLayerProtocol, IApplicationLayerProtocol applicationLayerProtocol)
        {
            this.TransportLayerProtocol = transportLayerProtocol;
            this.ApplicationLayerProtocol = applicationLayerProtocol;
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

        public void Close()
        {
            if (_SerialPort.IsOpen == true)
            {
                _SerialPort.Close();
            }
        }

        public async Task<ILayPackageSendResult> Send(IDeviceCommand command, CancellationToken cancellationToken)
        {
            var payloadSend = await Task.Run(() => ApplicationLayerProtocol.Pack(command));
            var payloadResponse = await Send(payloadSend, cancellationToken);
            var respose = await Task.Run(() => ApplicationLayerProtocol.Unpack(payloadResponse, command));
            return new LayPackageSendResult(respose, true);
        }



        //串口通信如何处理
        //https://stackoverflow.com/questions/53335736/c-sharp-await-event-and-timeout-in-serial-port-communication
        public async Task<byte[]> Send(byte[] content, CancellationToken cancellationToken)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            var jobTask = Task.Run(() => {
                // Organize critical sections around logical serial port operations somehow.
                lock (_LockObject)
                {
                    return SomeCommand(content);
                }
            });
            if (jobTask != await Task.WhenAny(jobTask, Task.Delay(10, cancellationToken)))
            {
                //Timeout
            }
            var response = await jobTask;
            // Process response.
            return response;
        }

        private byte[] SomeCommand(byte[] content)
        {
            // Assume serial port timeouts are set.
            _SerialPort.Write(content, 0, content.Length);
            int offset = 0;
            int count = TransportLayerProtocol.MiniumResponseLength; // Expected response length.
            byte[] buffer = new byte[count];
            while (count > 0)
            {
                var readCount = _SerialPort.Read(buffer, offset, count);
                offset += readCount;
                count -= readCount;
            }
            var countRest = TransportLayerProtocol.GetLengthFromHeader(buffer);
            byte[] rest = new byte[countRest.Value];
            while (countRest > 0)
            {
                var readCount = _SerialPort.Read(rest, offset, countRest.Value);
                offset += readCount;
                countRest -= readCount;
            }
            byte[] whole = new byte[countRest.Value + count];
            Array.Copy(buffer, whole, count);
            Array.Copy(rest, 0, whole, count, countRest.Value);
            return whole;
        }

        #region IDispose
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
