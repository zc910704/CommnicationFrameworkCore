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
        public bool IsOpen { get => _TcpClient.Connected; }

        public ITransportLayerProtocol TransportLayerProtocol { get; }

        public IApplicationLayerProtocol ApplicationLayerProtocol { get; }

        private TcpClient _TcpClient = null;

        /// <summary>
        /// 用于异步的信号量锁,限制最大访问的线程数
        /// 和 Semaphore 作用类似, SemaphoreSlim 有异步等待方法，支持在异步代码中线程同步。
        /// </summary>
        private SemaphoreSlim _WriteLock = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _ReadLock = new SemaphoreSlim(1, 1);

        private AutoResetEvent _DataArrive = new AutoResetEvent(false);


        private CancellationTokenSource _TokenSource = new CancellationTokenSource();
        private CancellationToken _Token;
        private Task _DataReceiver = null;
        private Task _MonitorSyncResponses = null;


        private object _LockObject = new object();

        private bool disposedValue;

        private List<byte> _Buffers = new List<byte>();

        public int ConnectTimeout { get; set; } = 1000;

        public int ReceiveTimeout { get; set; } = 1000;

        private readonly object _SyncResponseLock = new object();
        private Dictionary<UInt16, Response> _SyncResponses = new Dictionary<UInt16, Response>();

        public EthernetDevice(ITransportLayerProtocol transportLayerProtocol, IApplicationLayerProtocol applicationLayerProtocol)
        {
            this.TransportLayerProtocol = transportLayerProtocol;
            this.ApplicationLayerProtocol = applicationLayerProtocol;
        }

        public void Open()
        {
            if (IsOpen) throw new InvalidOperationException("Already Open connected to the server.");
            if (IsOpen == false)
            {
                _TcpClient = new TcpClient(AddressFamily.InterNetwork);
                /* 
                LingerState包含有关套接字逗留时间的信息。套接字逗留时间是指如果在套接字关闭后仍有数据要发送，套接字保持的时间量。
                套接字逗留时间的时间长度受与Socket实例相关联的LingerOption实例的设置控制。

                如果 Enabled 为false，调用Close方法将立即关闭与网络的连接。如果 Enabled 为 true，则数据将继续发送到网络上，
                但有 LingerTime 超时限制（以秒为单位）。发送完数据或超时过期时，网络连接会平稳关闭。如果发送队列中没有任何数据，套接字将立即关闭。
                当 Enabled 为 true 并且 LingerTime 为 0 时，调用 Close 会立即关闭套接字，所有未发送的数据都将丢失。
                */
                _TcpClient.LingerState = new LingerOption(true, 0);

                if (DeviceConfig == null) throw new NullReferenceException(nameof(DeviceConfig));
                if (DeviceConfig is EthernetConfig cfg)
                {
                    if (this.IsOpen) Close();
                    var ipaddr = cfg.IpAddress;
                    var port = cfg.Port;
                    IPAddress ip;
                    try
                    {
                        ip = IPAddress.Parse(ipaddr);
                    }
                    catch (Exception ex)
                    when (ex is FormatException || ex is ArgumentNullException)
                    {
                        throw new ArgumentException($"{cfg.IpAddress} is not a reasonable ip address.");
                    }                  
                    _TcpClient.ReceiveTimeout = ReceiveTimeout;
                    var asyncResult = _TcpClient.BeginConnect(ip, port, null, null);
                    var waitHandle = asyncResult.AsyncWaitHandle;
                    try
                    {
                        var connectSuccess = waitHandle.WaitOne(ConnectTimeout, false);
                        if (!connectSuccess)
                        {
                            _TcpClient.Close();
                            throw new TimeoutException($"Timeout connecting to {ip}:{port}");
                        }
                        _TcpClient.EndConnect(asyncResult);
                        EnableKeepalives();
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    finally
                    {
                        waitHandle.Close();
                    }
                }
                else throw new InvalidCastException($"InvalidCast {nameof(DeviceConfig)} to type EthernetConfig");
                
                _TokenSource = new CancellationTokenSource();
                _Token = _TokenSource.Token;

                _DataReceiver = Task.Run(() => DataReceiver(_Token), _Token);
            }
        }

        public void Close()
        {
            if (IsOpen == true)
            {
                _TcpClient.Close();
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
            await _WriteLock.WaitAsync();
            _TcpClient.GetStream().Write(content);
            var packedContent = TransportLayerProtocol.Pack(content);
            var seq = TransportLayerProtocol.GetPackageSeq(packedContent);
            while (_SyncResponses.ContainsKey(seq) == false)
            {
                _DataArrive.WaitOne();
            }
            var reult = _SyncResponses[seq];
            return reult.Payload;
        }

        #region Send Data

        #endregion

        #region Receive Data
        private async Task DataReceiver(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (IsOpen == false) throw new InvalidOperationException("already disconnect from server.");
                await _ReadLock.WaitAsync();
                #region read header
                byte[] header = new byte[TransportLayerProtocol.MiniumResponseLength];
                var buff = await ReadAsync(TransportLayerProtocol.MiniumResponseLength, cancellationToken);
                var headerOffset = TransportLayerProtocol.FindHeaderOffset(buff);
                while (headerOffset == -1)
                {
                    buff = await ReadAsync(TransportLayerProtocol.MiniumResponseLength, cancellationToken);
                }
                if (headerOffset != 0)
                {
                    int count = TransportLayerProtocol.MiniumResponseLength - headerOffset;
                    var buff_rest = await ReadAsync(count, cancellationToken);
                    
                    Array.Copy(buff, header, buff.Length);
                    Array.Copy(buff_rest, 0, header, buff.Length, buff_rest.Length);
                }
                #endregion
                #region read body
                var countRest = TransportLayerProtocol.GetLengthFromHeader(header);
                if (countRest == null) throw new ArgumentNullException(nameof(countRest));

                byte[] rest = new byte[countRest.Value];
                rest = await ReadAsync(countRest.Value, cancellationToken);
                byte[] whole = new byte[rest.Length + header.Length];
                Array.Copy(header, whole, header.Length);
                Array.Copy(rest, 0, whole, header.Length, rest.Length);
                var seq = TransportLayerProtocol.GetPackageSeq(whole);
                var payload = TransportLayerProtocol.Unpack(whole);
                var response = new Response()
                {
                    DateTime = DateTime.Now,
                    Payload = payload
                };
                #endregion
                _SyncResponses.Add(seq, response);
                _DataArrive.Set();
                _ReadLock.Release();
            }
        }

        private async Task<byte[]> ReadAsync(int count, CancellationToken cancellationToken)
        {
            byte[] buff = new byte[count];
            int offset = 0;
            while (count > 0)
            {
                var readCount = await _TcpClient.GetStream().ReadAsync(buff, offset, count, cancellationToken);
                offset += readCount;
                count -= readCount;
            }
            return buff;
        }
        #endregion

        #region Private Function
        /// <summary>
        /// https://github.com/jchristn/WatsonTcp
        /// </summary>
        private void EnableKeepalives()
        {
            try
            {
#if NETCOREAPP || NET5_0

                _Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                _Client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, _Keepalive.TcpKeepAliveTime);
                _Client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, _Keepalive.TcpKeepAliveInterval);
                _Client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, _Keepalive.TcpKeepAliveRetryCount);

#elif NETFRAMEWORK

                byte[] keepAlive = new byte[12]; 
                Buffer.BlockCopy(BitConverter.GetBytes((uint)1), 0, keepAlive, 0, 4); 
                Buffer.BlockCopy(BitConverter.GetBytes((uint)_Keepalive.TcpKeepAliveTime), 0, keepAlive, 4, 4);  
                Buffer.BlockCopy(BitConverter.GetBytes((uint)_Keepalive.TcpKeepAliveInterval), 0, keepAlive, 8, 4);  
                _Client.Client.IOControl(IOControlCode.KeepAliveValues, keepAlive, null);

#elif NETSTANDARD

#endif
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

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
                _TcpClient.Close();
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

        public class Response
        { 
            public byte[] Payload { get; set; }

            public DateTime DateTime { get; set; }
        }
    }
}
