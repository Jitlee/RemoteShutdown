using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    public class ServiceClient : ServiceClientBase
    {
        public ServiceClient(string hostname, int port)
            : base(new TcpChannel(hostname, port))
        {
            
        }

        public ServiceClient(IChannel channel)
            : base(channel)
        {
        }
    }
}