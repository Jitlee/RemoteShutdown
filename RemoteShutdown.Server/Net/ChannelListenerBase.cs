using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    public abstract class ChannelListenerBase : CommunictionObject, IChannelListener
    {
        protected abstract IAsyncResult OnBeginAcceptChannel(AsyncCallback callback, object state);

        protected abstract IChannel OnEndAcceptChannel(IAsyncResult result);

        public IAsyncResult BeginAcceptChannel(AsyncCallback callback, object state)
        {
            return OnBeginAcceptChannel(callback, state);
        }

        public IChannel EndAcceptChannel(IAsyncResult result)
        {
            return OnEndAcceptChannel(result);
        }
    }
}
