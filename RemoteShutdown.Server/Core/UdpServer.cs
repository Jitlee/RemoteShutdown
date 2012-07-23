using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using RemoteShutdown.Net;
using RemoteShutdown.Utilities;

namespace RemoteShutdown.Server.Core
{
    public class UdpServer
    {
        private Socket _server = null;

        private ILogger _logger = LoggerFactory.GetLogger(typeof(UdpServer).FullName);

        private byte[] _buffer = new byte[Common.UdpBufferSize];

        public void Start()
        {
            if (null == _server)
            {
                try
                {
                    _server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    var iep = new IPEndPoint(IPAddress.Any, Common.ServerPort);
                    _server.Bind(iep);
                    BeginReceiveFrom();
                    Broadcast();
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

        public void Stop()
        {
            if (null != _server)
            {
                try
                {
                    _server.Close();
                    _server = null;
                }
                catch (SocketException socketException)
                {
                    _logger.Trance("[Stop] SocketException : {0}", socketException.Message);
                }
                catch (Exception exception)
                {
                    _logger.Debug("[Stop] Exception : {0}", exception.Message);
                }
            }
        }

        private void Broadcast()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                try
                {
                    IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, Common.ClientPort);

                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

                    var buffer = Encoding.UTF8.GetBytes("server");

                    socket.SendTo(buffer, iep);

                    buffer = null;
                }
                catch (SocketException socketException)
                {
                    _logger.Trance("[Broadcast] SocketException : {0}", socketException.Message);
                }
                catch (Exception exception)
                {
                    _logger.Debug("[Broadcast] Exception : {0}", exception.Message);
                }
            }
        }

        private void BeginReceiveFrom()
        {
            if (null != _server)
            {
                try
                {
                    var iep = new IPEndPoint(IPAddress.Any, Common.ClientPort);
                    var ep = (EndPoint)iep;
                    _server.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref ep, BeginReceiveFromCallback, null);
                }
                catch (SocketException socketException)
                {
                    _logger.Trance("[BeginReceiveFrom] SocketException : {0}", socketException.Message);
                }
                catch (Exception exception)
                {
                    _logger.Debug("[BeginReceiveFrom] Exception : {0}", exception.Message);
                }
            }
        }

        private void BeginReceiveFromCallback(IAsyncResult result)
        {
            try
            {
                var iep = new IPEndPoint(IPAddress.Any, Common.ClientPort);
                var ep = (EndPoint)iep;
                var size = _server.EndReceiveFrom(result, ref ep);
                if (Encoding.UTF8.GetString(_buffer, 0, size) == "client")
                {
                    Request(ep);
                }
                BeginReceiveFrom();
            }
            catch (SocketException socketException)
            {
                _logger.Trance("[BeginReceiveFromCallback] SocketException : {0}", socketException.Message);
            }
            catch (Exception exception)
            {
                _logger.Debug("[BeginReceiveFromCallback] Exception : {0}", exception.Message);
            }
        }

        private void Request(EndPoint ep)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                try
                {

                    IPEndPoint iep = ep as IPEndPoint;

                    iep.Port = Common.ClientPort;

                    var buffer = Encoding.UTF8.GetBytes("server");

                    socket.SendTo(buffer, iep);

                    buffer = null;
                }
                catch (SocketException socketException)
                {
                    _logger.Trance("[Broadcast] SocketException : {0}", socketException.Message);
                }
                catch (Exception exception)
                {
                    _logger.Debug("[Broadcast] Exception : {0}", exception.Message);
                }
            }
        }
    }
}
