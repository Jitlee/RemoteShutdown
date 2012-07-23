using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    public class AsyncReceiveEventArgs : AsyncEventArgs
    {
        public byte[] Buffer { get; private set; }

        public AsyncReceiveEventArgs(IChannel source, byte[] buffer)
            : base(source)
        {
            Buffer = buffer;
        }
    }
}
