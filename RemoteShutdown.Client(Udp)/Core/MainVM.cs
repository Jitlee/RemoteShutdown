using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RemoteShutdown.Core;
using RemoteShutdown.Net;
using RemoteShutdown.Utilities;
using RemoteShutdown.Views;

namespace RemoteShutdown.Client.Core
{
    public class MainVM : EntityObject
    {
        #region 变量

        private static MainVM _instance = new MainVM();

        private TcpClient _tcpClient = null;

        private UdpClient _udpClient = null;

        #endregion

        #region 属性

        public static MainVM Instance { get { return _instance; } }

        #endregion

        #region 构造方法

        private MainVM()
        {
#if DEBUG
            LoggerFactory.SetLoggerLevel(LoggerLevel.Trance);
#else
            LoggerFactory.SetLoggerInstance(typeof(MyLogger));
#endif
            _udpClient = new UdpClient();
            _udpClient.RequestAction = ServerRequest;
            _udpClient.Start();


            _tcpClient = new TcpClient();
            _tcpClient.ClosedAction = ServerClosed;
            _tcpClient.FaultAction = ServerFaulted;
            _tcpClient.ReceivedAction = Received;
        }

        #endregion

        #region 私有方法

        private void ServerRequest(string ipAddress)
        {
            _udpClient.Stop();

            if (_tcpClient.Connect(ipAddress, Common.Port))
            {
                var client = new ClientModel();
                client.HostName = Environment.MachineName;
                client.IPAddress = Common.GetLocalIPAddress();
                var xml = SerializerHelper.SerializerObject<ClientModel>(client);
                var buffer = Encoding.UTF8.GetBytes(xml);
                var flag = 1001;
                _tcpClient.Send(flag, buffer);
            }
            else
            {
                _udpClient.Start();
            }
        }

        private void ServerFaulted(Exception ex)
        {
            _udpClient.Start();
        }

        private void ServerClosed()
        {
            _udpClient.Start();
        }

        private void Received(byte[] buffer)
        {
            if(buffer.Length >=4)
            {
                var flag = BitConverter.ToInt32(buffer, 0);
                switch (flag)
                {
                    case 0: // 注销
                        if (SettingVM.Instance.AllowControl)
                        {
                            PowerHelper.Logoff();
                        }
                        break;
                    case 1: // 关机
                        if (SettingVM.Instance.AllowControl)
                        {
                            PowerHelper.Shutdown();
                        }
                        break;
                    case 2: // 重启
                        if (SettingVM.Instance.AllowControl)
                        {
                            PowerHelper.Reboot();
                        }
                        break;
                    case 998:
                    case 999: // 消息
                        if (SettingVM.Instance.AllowBroadcast)
                        {
                            var msg = Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4);
                            PopupMessageWindow.Show(msg);
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
