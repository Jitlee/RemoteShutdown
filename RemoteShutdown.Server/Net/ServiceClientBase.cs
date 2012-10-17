using System;

namespace RemoteShutdown.Net
{
    public abstract class ServiceClientBase : CommunictionObject, IChannel, IDisposable
    {
        private IChannel _channel;
        private ILogger _logger;

        public ServiceClientBase(IChannel channel)
        {
            _channel = channel;
        }

        protected override void OnOpen()
        {
            GetLogger().Trance("[OnOpen] Called.");
            _channel.Opened += OnOpened;
            _channel.Received += Received;
            _channel.Closed += OnClosed;
            _channel.Faulted += OnFaulted;
            _channel.Open();
        }

        protected override void OnClose()
        {
            GetLogger().Trance("[OnOpen] OnClose.");
            _channel.Close();
            _channel.Opened -= OnOpened;
            _channel.Received -= Received;
            _channel.Closed -= OnClosed;
            _channel.Faulted -= OnFaulted;
        }

        protected override System.IAsyncResult OnBeginOpen(System.AsyncCallback callback, object state)
        {
            GetLogger().Trance("[OnBeginOpen] Called.");
            _channel.Opened += OnOpened;
            _channel.Received += Received;
            _channel.Closed += OnClosed;
            _channel.Faulted += OnFaulted;
            return _channel.BeginOpen(callback, state);
        }

        protected override System.IAsyncResult OnBeginClose(System.AsyncCallback callback, object state)
        {
            return _channel.BeginClose(callback, state);
        }

        protected override void OnEndOpen(System.IAsyncResult result)
        {
            GetLogger().Trance("[OnEndOpen] OnClose.");
            _channel.EndOpen(result);
        }

        protected override void OnEndClose(System.IAsyncResult result)
        {
            _channel.EndClose(result);
        }

        private void OnOpened(object sender, EventArgs e)
        {
            GetLogger().Trance("[OnOpened] Called.");
            //_channel.BeginReceive();
            //base.OnOpened();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            GetLogger().Trance("[OnClosed] Called.");
            base.OnClosed();
        }

        private void OnFaulted(object sender, EventArgs e)
        {
            base.Fault(_channel.GetLastError());
        }

        public virtual void BeginSend(byte[] buffer)
        {
            _channel.BeginSend(buffer);
        }

        public virtual void Send(byte[] buffer)
        {
            _channel.Send(buffer);
        }

        public void BeginReceive()
        {
            _channel.BeginReceive();
        }

        public byte[] Receive()
        {
            return _channel.Receive();
        }

        public event System.EventHandler<AsyncReceiveEventArgs> Received;

        private void BeginOpenCallback(IAsyncResult result)
        {
            _channel.EndOpen(result);
        }

        private ILogger GetLogger()
        {
            return _logger ?? (_logger = LoggerFactory.GetLogger(typeof(ChannelBase).FullName));
        }

        void IDisposable.Dispose()
        {
            base.Close();
        }
    }
}
