using System;

namespace RemoteShutdown.Net
{
    public abstract class ChannelBase : CommunictionObject, IChannel, IDisposable
    {
        protected abstract void OnSend(byte[] buffer);

        protected abstract void OnBeginSend(byte[] buffer);

        protected abstract void OnBeginReceive();

        protected abstract byte[] OnReceive();

        private ILogger _logger;

        public void BeginSend(byte[] buffer)
        {
            OnBeginSend(buffer);
        }

        public void Send(byte[] buffer)
        {
            OnSend(buffer);
        }

        public event EventHandler<AsyncReceiveEventArgs> Received;

        public void BeginReceive()
        {
            OnBeginReceive();
        }

        protected void OnReceived(byte[] buffer)
        {
            if (Received != null)
            {
                try
                {
                    Received(this, new AsyncReceiveEventArgs(this, buffer));
                }
                catch (Exception exception)
                {
                    buffer = null;
                    GetLogger().Error("[OnReceived] : {0}", exception.Message);
                }
            }
            else
            {
                buffer = null;
            }
        }

        public byte[] Receive()
        {
            return OnReceive();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private ILogger GetLogger()
        {
            return _logger ?? (_logger =LoggerFactory.GetLogger(typeof(ChannelBase).FullName));
        }

        void IDisposable.Dispose()
        {
            base.Close();
        }
    }
}
