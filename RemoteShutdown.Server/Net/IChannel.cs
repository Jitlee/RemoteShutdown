using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    public interface IChannel : ICommunicationObject
    {
        void Send(byte[] buffer);
        void BeginSend(byte[] buffer);
        void BeginReceive();
        byte[] Receive();
        event EventHandler<AsyncReceiveEventArgs> Received;
    }
}
