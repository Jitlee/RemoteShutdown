using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RemoteShutdown.Net
{
    public class ServiceHost : ServiceHostBase
    {
        private ILogger _logger;

        public ServiceHost(IChannelListener channelListener)
            : base(channelListener)
        {
        }
        public ServiceHost(int port)
            : base(new TcpChannelListener(new IPEndPoint(IPAddress.Any, port)))
        {
        }

        private ILogger GetLogger()
        {
            return _logger ?? (_logger = LoggerFactory.GetLogger(typeof(ServiceHost).FullName));
        }
    }
}
