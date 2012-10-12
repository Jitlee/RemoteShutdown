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

        #region 公共方法

        public void Send(int flag, byte[] buffer)
        {
            _tcpClient.Send(flag, buffer);
        }

        #endregion

        #region 私有方法

        private static void SetFlag(byte[] dst, int flag, int index)
        {
            dst[index] = (byte)(flag);
            dst[index + 1] = (byte)(flag >> 8);
            dst[index + 2] = (byte)(flag >> 16);
            dst[index + 3] = (byte)(flag >> 24);
        }

        private void ServerRequest(string ipAddress)
        {
            _udpClient.Stop();

            if (_tcpClient.Connect(ipAddress, Common.Port))
            {
                var client = new ClientModel();
                client.HostName = SettingVM.Instance.HostName;
                client.IPAddress = Common.GetLocalIPAddress();
                var xml = SerializerHelper.SerializerObject<ClientModel>(client);
                var buffer = Encoding.UTF8.GetBytes(xml);
                var flag = Constants.CONNECT_FLAG;
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
                    case Constants.LOG_OFF_FLAG: // 注销
                        if (SettingVM.Instance.AllowControl)
                        {
                            PowerHelper.Logoff();
                        }
                        break;
                    case Constants.SHUTDOWN_FLAG: // 关机
                        if (SettingVM.Instance.AllowControl)
                        {
                            PowerHelper.Shutdown();
                        }
                        break;
                    case Constants.REBOOT_FLAG: // 重启
                        if (SettingVM.Instance.AllowControl)
                        {
                            PowerHelper.Reboot();
                        }
                        break;
                    case Constants.SEND_MESSAGE_TO_FLAG:
                    case Constants.SEND_MESSAGE_TO_ALL_FLAG: // 消息
                        if (SettingVM.Instance.AllowBroadcast)
                        {
                            var msg = Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4);
                            PopupMessageWindow.Show(msg);
                        }
                        break;
                    case Constants.MODIFY_HOSTNAME_FLAG:
                        var hostname = Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4);
                        SettingVM.Instance.HostName = hostname;
                        break;
                }
            }
        }

        #endregion
    }
}
