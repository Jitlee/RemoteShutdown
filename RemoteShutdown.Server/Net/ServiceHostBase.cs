using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    public abstract class ServiceHostBase : CommunictionObject, IDisposable
    {
        public event EventHandler<AsyncReceiveEventArgs> Received;

        public event EventHandler<AsyncEventArgs> Disconnect;

        private IChannelListener _channelListener;

        private List<IChannel> channels;

        private ILogger _logger;

        protected ServiceHostBase(IChannelListener channelListener)
        {
            channels = new List<IChannel>();
            _channelListener = channelListener;
        }

        protected override void OnOpen()
        {
            _channelListener.Opened += OnOpend;
            _channelListener.Faulted += OnFaulted;

            GetLogger().Trance("[OnOpen] : Called.");
            _channelListener.Open();
        }

        protected override void OnOpened()
        {
            GetLogger().Trance("[OnOpened] : Called.");
            base.OnOpened();
            OnAcceptChannel();
        }

        protected override void OnClose()
        {
            GetLogger().Trance("[OnClose] : Called.");
            _channelListener.Opened -= OnOpend;
            _channelListener.Faulted -= OnFaulted;
            _channelListener.Close();
            channels.ForEach((channel) =>
            {
                this.OnRemoveChannel(channel);
            });
            channels.Clear();
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

        private void OnOpend(object sender, EventArgs e)
        {
            base.OnOpened();
        }

        private void OnAcceptChannel()
        {
            try
            {
                GetLogger().Trance("[OnAcceptChannel] : Called.");
                var asyncResult = _channelListener.BeginAcceptChannel(AcceptChannelCallback, null);
                if (asyncResult.CompletedSynchronously)
                {
                    AcceptChannelCallback(asyncResult);
                }
            }
            catch (Exception ex)
            {
                GetLogger().Error("[OnAcceptChannel] : {0}", ex.Message);
                base.Fault(ex);
            }
        }

        private void AcceptChannelCallback(IAsyncResult result)
        {
            try
            {
                GetLogger().Trance("[AcceptChannelCallback] : Called.");
                var channel = _channelListener.EndAcceptChannel(result);
                if (null != channel)
                {
                    if (this.State == CommunicationState.Closed)
                    {
                        throw new ObjectDisposedException(this.GetType().ToString());
                    }
                    channels.Add(channel);
                    OnAddChannel(channel);
                    OnAcceptChannel();
                }
            }
            catch (Exception ex)
            {
                GetLogger().Error("[AcceptChannelCallback] : {0}", ex.Message);
                base.Fault(ex);
            }
        }

        internal void OnAddChannel(IChannel channel)
        {
            GetLogger().Trance("[OnAddChannel] called.");
            channel.Received += Received;
            channel.Faulted += OnChannelFaulted;
            channel.Closed += OnChannelClosed;
            channel.BeginReceive();
        }

        internal void OnRemoveChannel(IChannel channel)
        {
            GetLogger().Trance("[OnRemoveChannel] called.");
            OnDisconnect(this, new AsyncEventArgs(channel));
            channel.Close();
            channel.Received -= Received;
            channel.Faulted -= OnChannelFaulted;
            channel.Closed -= OnChannelClosed;
        }

        private void OnDisconnect(object sender, AsyncEventArgs e)
        {
            if (null != Disconnect)
            {
                Disconnect(sender, e);
            }
        }

        private void OnChannelFaulted(object sender, EventArgs e)
        {
            var channel = sender as IChannel;
            if (null != channel && channels.Contains(channel))
            {
                channels.Remove(channel);
                this.OnRemoveChannel(channel);
            }
        }

        private void OnChannelClosed(object sender, EventArgs e)
        {
            var channel = sender as IChannel;
            if (null != channel && channels.Contains(channel))
            {
                channels.Remove(channel);
                this.OnRemoveChannel(channel);
            }
        }

        private void OnFaulted(object sender, EventArgs e)
        {
            base.Fault(_channelListener.GetLastError());
        }

        private ILogger GetLogger()
        {
            return _logger ?? (_logger = LoggerFactory.GetLogger(typeof(ServiceHostBase).FullName));
        }

        void IDisposable.Dispose()
        {
            base.Close();
        }
    }
}
