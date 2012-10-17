using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RemoteShutdown.Core;
using RemoteShutdown.Net;
using RemoteShutdown.Utilities;
using System.Threading;
using RemoteShutdown.Views;
using System.Threading.Tasks;

namespace RemoteShutdown.Client.Core
{
    public class MainVM : EntityObject
    {
        #region 变量

        private static readonly MainVM _instance = new MainVM();

        private readonly TcpClient _tcpClient = null;

        private readonly UdpClient _udpClient = null;

        private bool? _isConnected;

        private readonly Timer _autoTimer;

        #endregion

        #region 属性

        public bool? IsConnected
        {
            get { return _isConnected; }
            private set
            {
                _isConnected = value;


                _autoTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _udpClient.Stop();

                if (value == false)
                {
                    _isConnected = null;
                    if (SettingVM.Instance.AutoAddress)
                    {
                        _udpClient.Start();
                    }
                    else
                    {
                        _autoTimer.Change(Common.AutoConnectInterval, Common.AutoConnectInterval);
                    }
                }
            }
        }

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

            _autoTimer = new Timer(AtuoTimerCallback, null, Timeout.Infinite, Timeout.Infinite);

            _tcpClient = new TcpClient();
            _tcpClient.OpenedAction = ServerOpened;
            _tcpClient.ClosedAction = ServerClosed;
            _tcpClient.FaultAction = ServerFaulted;
            _tcpClient.ReceivedAction = Received;

            if (SettingVM.Instance.AutoAddress)
            {
                _udpClient.Start();
            }
            else
            {
                _autoTimer.Change(Common.AutoConnectInterval, Common.AutoConnectInterval);
            }
        }

        #endregion

        #region 私有方法

        private void ServerRequest(string ipAddress)
        {
            Connect(ipAddress);
        }

        private void AtuoTimerCallback(object state)
        {
            if (_isConnected == true)
            {
                _autoTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else
            {
                Connect();
            }
        }

        private void ServerOpened()
        {
            IsConnected = true;

            var client = new ClientModel();
            client.HostName = SettingVM.Instance.HostName;
            client.IPAddress = Common.GetLocalIPAddress();
            var xml = SerializerHelper.SerializerObject<ClientModel>(client);
            var buffer = Encoding.UTF8.GetBytes(xml);
            var flag = Constants.CONNECT_FLAG;
            _tcpClient.Send(flag, buffer);
        }

        private void ServerFaulted(Exception ex)
        {
            if (null != _isConnected)
            {
                IsConnected = false;
            }
        }

        private void ServerClosed()
        {
            if (null != _isConnected)
            {
                IsConnected = false;
            }
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

        #region 公共方法

        public void Reset()
        {
            IsConnected = false;
            _tcpClient.Disconnect();
        }

        public void Connect()
        {
            var ipAddress = SettingVM.Instance.ServerAddress;
            Connect(ipAddress);
        }

        private void Connect(string ipAddress)
        {
            if (IsConnected != true)
            {
                _tcpClient.Connect(ipAddress, Common.Port);
            }
        }

        public void Send(int flag, byte[] buffer)
        {
            _tcpClient.Send(flag, buffer);
        }

        #endregion
    }
}
