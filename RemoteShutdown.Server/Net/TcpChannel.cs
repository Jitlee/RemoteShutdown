using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RemoteShutdown.Net
{
    public class TcpChannel : ChannelBase
    {
        const int BUFFER_SIZE = 1024;
        private TcpClient _client;

        private string _hostname;

        private int _port;

        private string _ipAddress = string.Empty;

        private bool _isShutdown = false;

        public TcpChannel(TcpClient client)
        {
            _client = client;
            if (_client.Connected)
            {
                State = CommunicationState.Opened;
                _ipAddress = (_client.Client.RemoteEndPoint as IPEndPoint).Address.ToString();
            }
        }

        public TcpChannel(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
            _client = new TcpClient();
        }

        private ILogger _logger;
        private ILogger GetLogger()
        {
            return _logger ?? (_logger = LoggerFactory.GetLogger(typeof(TcpChannel).FullName));
        }

        protected override void OnSend(byte[] buffer)
        {
            try
            {
                if (null != _client && null != _client.Client)
                {
                    _client.Client.Send(buffer);
                }
                else
                {
                    Close();
                }
            }
            catch (SocketException socketException)
            {
                GetLogger().Trance("[OnSend] SocketException : {0}", socketException.Message);
                Close();
                base.Fault(socketException);
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                GetLogger().Debug("[OnSend] ObjectDisposedException : {0}", objectDisposedException.Message);
                Close();
                base.Fault(objectDisposedException);
            }
            catch (Exception exception)
            {
                GetLogger().Error("[OnSend] ({0}) Exception : {1}", _ipAddress, exception.Message);
                Close();
                base.Fault(exception);
            }
        }

        protected override void OnBeginSend(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        protected override void OnBeginReceive()
        {
            var buffer = new byte[BUFFER_SIZE];
            try
            {
                if (null != _client && null != _client.Client)
                {
                    var asyncResult = _client.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, buffer);
                }
                else
                {
                    Close();
                }
            }
            catch (SocketException socketException)
            {
                GetLogger().Trance("[OnBeginReceive] SocketException : {0}", socketException.Message);
                Close();
                base.Fault(socketException);
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                GetLogger().Debug("[OnBeginReceive] ObjectDisposedException : {0}", objectDisposedException.Message);
                Close();
                base.Fault(objectDisposedException);
            }
            catch (Exception exception)
            {
                GetLogger().Error("[OnBeginReceive] ({0}) Exception : {1}", _ipAddress, exception.Message);
                Close();
                base.Fault(exception);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            if (base.State != CommunicationState.Opened)
            {
                return;
            }
            try
            {
                var asyncReadSize = _client.Client.EndReceive(result);

                if (asyncReadSize == 0)
                {
                    base.Close();
                    return;
                }

                var buffer = result.AsyncState as byte[];
                var dst = new byte[asyncReadSize];
                Buffer.BlockCopy(buffer, 0, dst, 0, dst.Length);
                buffer = null;
                base.OnReceived(dst);

                OnBeginReceive();
            }
            catch (SocketException socketException)
            {
                GetLogger().Trance("[ReceiveCallback] SocketException : {0}", socketException.Message);
                Close();
                base.Fault(socketException);
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                GetLogger().Debug("[ReceiveCallback] ObjectDisposedException : {0}", objectDisposedException.Message);
                Close();
                base.Fault(objectDisposedException);
            }
            catch (Exception exception)
            {
                GetLogger().Error("[ReceiveCallback] ({0}) Exception : {1}", _ipAddress, exception.Message);
                Close();
                base.Fault(exception);
            }
        }

        protected override byte[] OnReceive()
        {
            try
            {
                var buffer = new byte[BUFFER_SIZE];
                var bufferSize = _client.Client.Receive(buffer);
                var result = new byte[bufferSize];
                Buffer.BlockCopy(buffer, 0, result, 0, bufferSize);
                return result;
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                GetLogger().Debug("[ReceiveCallback] ObjectDisposedException : {0}", objectDisposedException.Message);
                Close();
                base.Fault(objectDisposedException);
                return new byte[0];
            }
            catch (Exception exception)
            {
                GetLogger().Error("[ReceiveCallback] ({0}) Exception : {1}", _ipAddress, exception.Message);
                Close();
                base.Fault(exception);
                return new byte[0];
            }
        }

        protected override void OnOpen()
        {
            _client.Connect(_hostname, _port);
        }

        protected override void OnClose()
        {
            GetLogger().Trance("[OnClose] Called.");
            Shutdown();
            _client.Close();
        }

        private void Shutdown()
        {
            //lock (ThisLock)
            //{
            if (_isShutdown)
            {
                return;
            }
            _isShutdown = true;
            //}

            try
            {
                _client.Client.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException socketException)
            {
                GetLogger().Trance("[Shutdown] SocketException : {0}", socketException.Message);
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                GetLogger().Trance("[Shutdown] ObjectDisposedException : {0}", objectDisposedException.Message);
            }
            catch (Exception ex)
            {
                GetLogger().Trance("[Shutdown] Exception : {0}", ex.Message);
            }
        }

        protected override IAsyncResult OnBeginOpen(AsyncCallback callback, object state)
        {
            try
            {
                return _client.BeginConnect(_hostname, _port, callback, state);
            }
            catch (SocketException socketException)
            {
                GetLogger().Trance("[OnBeginOpen] SocketException : {0}", socketException.Message);
                Close();
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                GetLogger().Trance("[OnBeginOpen] ObjectDisposedException : {0}", objectDisposedException.Message);
                Close();
            }
            catch (Exception ex)
            {
                GetLogger().Trance("[OnBeginOpen] Exception : {0}", ex.Message);
                Close();
            }
            return null;
        }

        protected override IAsyncResult OnBeginClose(AsyncCallback callback, object state)
        {
            throw new NotImplementedException("OnBeginClose");
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            try
            {
                _client.EndConnect(result);
            }
            catch (SocketException socketException)
            {
                GetLogger().Trance("[OnEndOpen] SocketException : {0}", socketException.Message);
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                GetLogger().Trance("[OnEndOpen] ObjectDisposedException : {0}", objectDisposedException.Message);
            }
            catch (Exception ex)
            {
                GetLogger().Trance("[OnEndOpen] Exception : {0}", ex.Message);
            }
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            throw new NotImplementedException("OnEndClose");
        }
    }
}
