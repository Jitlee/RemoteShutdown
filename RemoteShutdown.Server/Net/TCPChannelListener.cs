using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace RemoteShutdown.Net
{
    public class TcpChannelListener : ChannelListenerBase, IDisposable
    {
        private TcpListener _listener;

        private ILogger _logger;

        public TcpChannelListener(IPEndPoint ipAddress)
            : base()
        {
            _listener = new TcpListener(ipAddress);
        }

        protected override void OnOpen()
        {
            GetLogger().Trance("[OnOpen] : Called.");
            try
            {
                _listener.Start();
            }
            catch (Exception ex)
            {
                base.Fault(ex);
                GetLogger().Error("[OnOpen] : Exception : {0}", ex.Message);
            }
        }

        protected override void OnClose()
        {
            GetLogger().Trance("[OnClose] : Called.");
            try
            {
                try
                {
                    _listener.Server.Shutdown(SocketShutdown.Both);
                }
                catch { }
                _listener.Server.Close();
                _listener.Stop();
            }
            catch (ObjectDisposedException objectDisosedException)
            {
                GetLogger().Trance("[OnClose] : ObjectDisposedException : {0}", objectDisosedException.Message);
            }
            catch (Exception ex)
            {
                GetLogger().Trance("[OnClose] : Exception : {0}", ex.Message);
            }
        }

        protected override void OnFaulted()
        {
            GetLogger().Trance("[OnFaulted] : Called.");
            base.OnFaulted();
        }

        protected override IAsyncResult OnBeginOpen(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override IAsyncResult OnBeginClose(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected override IAsyncResult OnBeginAcceptChannel(AsyncCallback callback, object state)
        {
            try
            {
                return _listener.BeginAcceptTcpClient(callback, state);
            }
            catch (ObjectDisposedException objectDisosedException)
            {
                GetLogger().Error("[OnBeginAcceptChannel] : ObjectDisposedException : {0}", objectDisosedException.Message);
                throw objectDisosedException;
            }
            catch (Exception ex)
            {
                GetLogger().Error("[OnBeginAcceptChannel] : Exception : {0}", ex.Message);
                throw ex;
            }
        }

        protected override IChannel OnEndAcceptChannel(IAsyncResult result)
        {
            if (State != CommunicationState.Opened)
            {
                return null;
            }

            var client = _listener.EndAcceptTcpClient(result);
            GetLogger().Trance("[OnEndAcceptChannel] Calld : {0}({1}) is online",
                (client.Client.RemoteEndPoint as IPEndPoint).Address,
                (client.Client.RemoteEndPoint as IPEndPoint).Port);
            return new TcpChannel(client);
        }

        private ILogger GetLogger()
        {
            return _logger ?? (_logger = LoggerFactory.GetLogger(typeof(TcpChannelListener).FullName));
        }

        void IDisposable.Dispose()
        {
            base.Close();
        }
    }
}
