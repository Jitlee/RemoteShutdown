using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using RemoteShutdown.Net;
using RemoteShutdown.Utilities;

namespace RemoteShutdown.Client.Core
{
    public class TcpClient
    {
        private ServiceClient _client = null;

        private ILogger _logger = LoggerFactory.GetLogger(typeof(TcpClient).FullName);

        private readonly Timer _heartbeatTimer;

        private byte[] _heartbeatBuffer = new byte[] { 0x00 };

        private bool _isConnected;

        public Action<Exception> FaultAction { get; set; }

        public Action ClosedAction { get; set; }

        public Action<byte[]> ReceivedAction { get; set; }

        public bool IsConnected
        {
            get { return _isConnected; }
            private set
            {
                _isConnected = value;

                if (value)
                {
                    _heartbeatTimer.Change(Common.HeartbeatInterval, Common.HeartbeatInterval);
                }
                else
                {
                    _heartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        public TcpClient()
        {
            _heartbeatTimer = new Timer(HeartbeatTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void HeartbeatTimerCallback(object state)
        {
            Send(1003, _heartbeatBuffer);
        }

        public bool Connect(string ipAddress, int port)
        {
            Disconnect();

            try
            {
                _client = new ServiceClient(ipAddress, port);
                _client.Opened += Client_Opened;
                _client.Faulted += Client_Faulted;
                _client.Received += Client_Receive;
                _client.Closed += Client_Closed;
                _client.Open();
                return true;
            }
            catch (SocketException socketException)
            {
                Disconnect();
                _logger.Trance("[Connect] SocketException : {0}", socketException.Message);
            }
            catch (Exception exception)
            {
                Disconnect();
                _logger.Debug("[Connect] Exception : {0}", exception.Message);
            }
            return false;
        }

        public void Disconnect()
        {
            if (null != _client)
            {
                try
                {
                    _client.Opened -= Client_Opened;
                    _client.Faulted -= Client_Faulted;
                    _client.Received -= Client_Receive;
                    _client.Closed -= Client_Closed;
                    _client.Close();
                    _client = null;

                    IsConnected = false;
                }
                catch (SocketException socketException)
                {
                    _logger.Trance("[Disconnect] SocketException : {0}", socketException.Message);
                }
                catch (Exception exception)
                {
                    _logger.Debug("[Disconnect] Exception : {0}", exception.Message);
                }
            }
        }

        public void Send(int flag, byte[] buffer)
        {
            if (null != _client)
            {
                var dst = new byte[buffer.Length + 4];
                dst[0] = (byte)flag;
                dst[1] = (byte)(flag >> 8);
                dst[2] = (byte)(flag >> 16);
                dst[3] = (byte)(flag >> 24);
                Buffer.BlockCopy(buffer, 0, dst, 4, buffer.Length);
                _client.Send(dst);
                dst = null;
                buffer = null;
            }
        }

        private void Client_Opened(object sender, EventArgs e)
        {
            IsConnected = true;
            _client.BeginReceive();
        }

        private void Client_Faulted(object sender, EventArgs e)
        {
            IsConnected = false;
            var error = _client.GetLastError();
            if (null != error && null != FaultAction)
            {
                FaultAction(error);
            }
        }

        private void Client_Closed(object sender, EventArgs e)
        {
            IsConnected = false;
            if (null != ClosedAction)
            {
                ClosedAction();
            }
        }

        private void Client_Receive(object sender, AsyncReceiveEventArgs e)
        {
            if (null != ReceivedAction)
            {
                ReceivedAction(e.Buffer);
            }
        }
    }
}
