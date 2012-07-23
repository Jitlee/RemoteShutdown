using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    public class AsyncEventArgs : EventArgs
    {
        public IChannel Source { get; private set; }

        public AsyncEventArgs(IChannel source)
        {
            Source = source;
        }
    }
}
