using CommDeviceCore.Common;
using CommDeviceCore.Common.Interface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommDeviceCore.PhysicalCommDevice
{
    public class EthernetDevice : IPhyCommDevice
    {
        public IDeviceConfig DeviceConfig { get; set; }
        public bool IsOpen { get => socket.Connected; }

        public ITransportLayerProtocol TransportLayerProtocol { get; }

        public IApplicationLayerProtocol ApplicationLayerProtocol { get; }

        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private object _LockObject = new object();

        private bool disposedValue;

        private List<byte> _Buffers = new List<byte>();

        public EthernetDevice(ITransportLayerProtocol transportLayerProtocol, IApplicationLayerProtocol applicationLayerProtocol)
        {
            this.TransportLayerProtocol = transportLayerProtocol;
            this.ApplicationLayerProtocol = applicationLayerProtocol;
        }

        public void Open()
        {
            if (IsOpen == false)
            {
                if (DeviceConfig == null) throw new NullReferenceException(nameof(DeviceConfig));
                if (DeviceConfig is EthernetConfig cfg)
                {
                    if (this.IsOpen) Close();
                    var ipaddr = cfg.IpAddress;
                    var port = cfg.Port;
                    IPAddress ip = IPAddress.Parse(ipaddr);
                    IPEndPoint iep = new IPEndPoint(ip, port);
                    socket.ReceiveTimeout = 1000;
                    socket.Connect(iep);
                }
                else throw new InvalidCastException($"InvalidCast {nameof(DeviceConfig)} to type EthernetConfig");
            }
        }

        public void Close()
        {
            if (IsOpen == true)
            {
                socket.Close();
            }
        }

        public async Task<ILayPackageSendResult> Send(IDeviceCommand command, CancellationToken cancellationToken)
        {
            var payloadSend = await Task.Run(() => ApplicationLayerProtocol.Pack(command));
            var sendload = await Task.Run(() => TransportLayerProtocol.Pack(payloadSend));
            var payloadResponse = await Send(sendload, cancellationToken);
            var receiveload = await Task.Run(() => TransportLayerProtocol.Unpack(payloadResponse));
            _ = Task.Run(() => ApplicationLayerProtocol.Unpack(receiveload, command));
            return new LayPackageSendResult(command, true);
        }


        public async Task<byte[]> Send(byte[] content, CancellationToken cancellationToken)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            var jobTask = Task.Run(() => {
                lock (_LockObject)
                {
                    return SomeCommand(content);
                }
            });
            if (jobTask != await Task.WhenAny(jobTask, Task.Delay(50, cancellationToken)))
            {
                //Timeout
            }
            var response = await jobTask;
            // Process response.
            return response;
        }

        private async Task<byte[]> SomeCommand(byte[] content)
        {
            try
            {
                socket.Send(content);
                int offset = 0;
                int count = TransportLayerProtocol.MiniumResponseLength; // Expected response length.
                byte[] header = new byte[count];
                await Task.Run(() =>
                {
                    while (count > 0)
                    {
                        var readCount = socket.Receive(header, offset, count, SocketFlags.None);
                        offset += readCount;
                        count -= readCount;
                    }
                });
                var countRest = TransportLayerProtocol.GetLengthFromHeader(header);
                byte[] rest = new byte[countRest.Value];
                var offsetRest = 0;
                while (countRest > 0)
                {
                    var readCount = socket.Receive(rest, offsetRest, countRest.Value, SocketFlags.None);
                    offsetRest += readCount;
                    countRest -= readCount;
                }
                byte[] whole = new byte[rest.Length + header.Length];
                Array.Copy(header, whole, header.Length);
                Array.Copy(rest, 0, whole, header.Length, rest.Length);
                return whole;
            }
            catch (Exception ex) { }
            throw new NotSupportedException();
        }

        private byte[] ReceiveData()
        {
            byte[] buffer = new byte[512 * 4096];

            try
            {
                if (socket != null)
                {
                    int len = 0;
                    len = socket.Receive(buffer);
                    if (len > 0)
                    {
                        byte[] databuf = new byte[len];
                        Array.Copy(buffer, 0, databuf, 0, len);
                        return databuf;
                    }
                    else
                        Thread.Sleep(50);
                }
            }
            catch (Exception)
            {
            }

            return new byte[] { };
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
                socket.Close();
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
