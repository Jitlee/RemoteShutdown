using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using RemoteShutdown.Net;
using RemoteShutdown.Utilities;

namespace RemoteShutdown.Server.Core
{
    public class TCPServer
    {
        private ServiceHost _server = null;

        private ILogger _logger = LoggerFactory.GetLogger(typeof(TCPServer).FullName);

        public Action<IChannel, byte[]> ReceivedAction { get; set; }

        public Action<IChannel> DisconnectAction { get; set; }

        public void Start()
        {
            Stop();
            try
            {
                _server = new ServiceHost(Common.Port);
                _server.Received += Server_Received;
                _server.Disconnect += Server_Disconnect;
                _server.Open();
            }
            catch (SocketException socketException)
            {
                Stop();
                _logger.Trance("[Start] SocketException : {0}", socketException.Message);
            }
            catch (Exception exception)
            {
                Stop();
                _logger.Debug("[Start] Exception : {0}", exception.Message);
            }
        }

        public void Stop()
        {
            if (null != _server)
            {
                try
                {
                    _server.Received -= Server_Received;
                    _server.Disconnect -= Server_Disconnect;
                    _server.Close();
                    _server = null;
                }
                catch (SocketException socketException)
                {
                    _logger.Trance("[Start] SocketException : {0}", socketException.Message);
                }
                catch (Exception exception)
                {
                    _logger.Debug("[Start] Exception : {0}", exception.Message);
                }
            }
        }

        public void SendTo(IChannel channel, byte[] buffer)
        {
            channel.Send(buffer);
        }

        private void Server_Received(object sender, AsyncReceiveEventArgs e)
        {
            if (null != ReceivedAction)
            {
                ReceivedAction(e.Source, e.Buffer);
            }
        }

        private void Server_Disconnect(object sender, AsyncEventArgs e)
        {
            if (null != DisconnectAction)
            {
                DisconnectAction(e.Source);
            }
        }
    }
}
