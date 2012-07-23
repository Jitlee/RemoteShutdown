using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    public interface IChannelListener : ICommunicationObject
    {
        IAsyncResult BeginAcceptChannel(AsyncCallback callback, object state);
        IChannel EndAcceptChannel(IAsyncResult result);
    }
}
