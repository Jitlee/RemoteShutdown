using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using RemoteShutdown.Core;
using RemoteShutdown.Net;
using RemoteShutdown.Server.Views;
using RemoteShutdown.Utilities;

namespace RemoteShutdown.Server.Core
{
    public class MainVM : EntityObject
    {
        #region 变量

        public static MainVM _instance = new MainVM();

        private ILogger _logger = LoggerFactory.GetLogger(typeof(MainVM).FullName);

        private UdpServer _udpServer = new UdpServer();

        private TCPServer _tcpServer = new TCPServer();

        private readonly ObservableCollection<ClientModel> _items = new ObservableCollection<ClientModel>();

        private readonly DelegateCommand<object> _powerCommand;

        private readonly DelegateCommand<object> _powerAllCommand;

        private readonly DelegateCommand<string> _powerAllTimeCommand;

        private readonly DelegateCommand _sendMessageToCommand;

        private readonly DelegateCommand _sendMessageToAllCommand;

        private readonly DelegateCommand _renameCommand;

        private ClientModel _selectedClient;

        #endregion

        #region 属性

        public static MainVM Instance { get { return _instance; } }

        public ObservableCollection<ClientModel> Items { get { return _items; } }

        public DelegateCommand<object> PowerCommand { get { return _powerCommand; } }

        public DelegateCommand<object> PowerAllCommand { get { return _powerAllCommand; } }

        public DelegateCommand<string> PowerAllTimeCommand { get { return _powerAllTimeCommand; } }

        public DelegateCommand SendMessageToCommand { get { return _sendMessageToCommand; } }

        public DelegateCommand SendMessageToAllCommand { get { return _sendMessageToAllCommand; } }

        public DelegateCommand RenameCommand { get { return _renameCommand; } }

        public ClientModel SelectedClient {
            get
            {
                return _selectedClient;
            }
            set 
            { 
                _selectedClient = value;
                RaisePropertyChanged("SelectedClient");
                _powerCommand.RaiseCanExecuteChanged();
                _sendMessageToCommand.RaiseCanExecuteChanged();
                _renameCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region 构造方法

        private MainVM()
        {
#if DEBUG
            LoggerFactory.SetLoggerLevel(LoggerLevel.Trance);
#else
            LoggerFactory.SetLoggerInstance(typeof(MyLogger));
#endif

            _powerCommand = new DelegateCommand<object>(Power, CanPower);

            _powerAllCommand = new DelegateCommand<object>(PowerAll, CanPowerAll);

            _powerAllTimeCommand = new DelegateCommand<string>(PowerTimeAll, CanPowerAll);

            _sendMessageToCommand = new DelegateCommand(SendMessageTo, CanSendMessageTo);

            _sendMessageToAllCommand = new DelegateCommand(SendMessageToAll, CanSendMessageToAll);

            _renameCommand = new DelegateCommand(Rename, CanRename);

            _tcpServer.ReceivedAction = Received;

            _tcpServer.DisconnectAction = DisConnected;

            _tcpServer.Start();

            _udpServer.Start();

        }

        #endregion

        #region 私有方法

        private void Received(IChannel channel, byte[] buffer)
        {
            if (buffer.Length > 4)
            {
                var flag = BitConverter.ToInt32(buffer, 0);
                switch (flag)
                {
                    case Constants.CONNECT_FLAG:  // 连接
                        Connected(channel, buffer);
                        break;
                    case Constants.MODIFY_HOSTNAME_FLAG:  // 修改终端名
                        ModifyHostName(channel, buffer);
                        break;
                }
            }
        }

        private void Connected(IChannel channel, byte[] buffer)
        {
            try
            {
                var xml = Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4);
                var client = SerializerHelper.DeserializeObject<ClientModel>(xml);
                if (null != client)
                {
                    client.Channel = channel;
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        _items.Add(client);

                        _powerAllCommand.RaiseCanExecuteChanged();
                        _powerAllTimeCommand.RaiseCanExecuteChanged();
                        _sendMessageToAllCommand.RaiseCanExecuteChanged();

                    }));
                }
            }
            catch (Exception exception)
            {
                _logger.Debug("[Connected] Exception : {0}", exception.Message);
            }
        }

        private void ModifyHostName(IChannel channel, byte[] buffer)
        {
            try
            {
                var hostname = Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4);
                var client = _items.FirstOrDefault(c => c.Channel == channel);
                if (null != client)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        client.HostName = hostname;
                    }));
                }
            }
            catch (Exception exception)
            {
                _logger.Debug("[ModifyHostName] Exception : {0}", exception.Message);
            }
        }

        private void DisConnected(IChannel channel)
        {
            var client = _items.FirstOrDefault(c => c.Channel == channel);
            if (null != client)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    _items.Remove(client);
                    _powerAllCommand.RaiseCanExecuteChanged();
                    _powerAllTimeCommand.RaiseCanExecuteChanged();
                    _sendMessageToAllCommand.RaiseCanExecuteChanged();
                }));
            }
        }

        private void Power(object parameter)
        {
            var powerType = Converter.ToInt(parameter);
            if (null != _selectedClient)
            {
                _tcpServer.SendTo(_selectedClient.Channel, BitConverter.GetBytes(powerType));
            }
        }

        private bool CanPower()
        {
            return null != _selectedClient;
        }

        private void PowerAll(object parameter)
        {
            var powerType = Converter.ToInt(parameter);
            var powerText = powerType == 0 ? ResourcesHelper.GetValue("LogoffString", "注销") : (powerType == 1 ? ResourcesHelper.GetValue("ShutdownString", "关机") : ResourcesHelper.GetValue("RebootString", "重启"));
            var buffer = BitConverter.GetBytes(powerType);
            if (MessageBox.Show(Application.Current.MainWindow,
                string.Format(ResourcesHelper.GetValue("ExcuteAllString", "是否确定执行全部{0}命令?"), powerText),
                ResourcesHelper.GetValue("QueryString", "询问"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                foreach (var client in _items)
                {
                    client.Channel.Send(buffer);
                }
            }
        }

        private void PowerTimeAll(string dateTime)
        {
            try
            {
                DateTime temp =DateTime.Parse(dateTime);
                if (DateTime.Now.CompareTo(temp) > 0)
                {
                    MessageBox.Show("您输入的时间不能小于当前时间！");
                }
                else
                {
                    MessageBox.Show("设置成功，请等待......！");
                }

                Timer time = new Timer();
                time.Interval = 60000;
                time.Enabled = true;
                time.Elapsed+=new ElapsedEventHandler(
                    (objec,ee)=>
                    {
                        Application.Current.Dispatcher.Invoke(
                    (Action)delegate
                {

                    if (DateTime.Now.CompareTo(temp) < 0)
                    {
                        PowerAll(Constants.SHUTDOWN_FLAG);
                    }


                });
                
                });
                time.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("您输入的时间格式不正确！");
            }
        }


        private bool CanPowerAll()
        {
            return null != _items && _items.Count > 0;
        }

        private void SendMessageTo()
        {
            if (null != _selectedClient)
            {
                var client = _selectedClient;
                var sendMessageWindow = new SendMessageWindow();

                sendMessageWindow.Title = string.Format("{0}({1}: {2})",
                    ResourcesHelper.GetValue("SendMessageString", "发送消息"),
                    ResourcesHelper.GetValue("ToString", "给"),
                    client.IPAddress);
                if(sendMessageWindow.ShowDialog() == true)
                {
                    var message = sendMessageWindow.MessageTextBox.Text;
                    var buffer = Encoding.UTF8.GetBytes(message);
                    var dst = new byte[buffer.Length + 4];
                    var flag = Constants.SEND_MESSAGE_TO_FLAG;
                    SetFlag(dst, flag, 0);
                    Buffer.BlockCopy(buffer, 0, dst, 4, buffer.Length);
                    buffer = null;
                    _tcpServer.SendTo(client.Channel, dst);
                    dst = null;
                }
            }
        }

        private static void SetFlag(byte[] dst, int flag, int index)
        {
            dst[index] = (byte)(flag);
            dst[index + 1] = (byte)(flag >> 8);
            dst[index + 2] = (byte)(flag >> 16);
            dst[index + 3] = (byte)(flag >> 24);
        }

        private bool CanSendMessageTo()
        {
            return null != _selectedClient;
        }

        private void SendMessageToAll()
        {
            var sendMessageWindow = new SendMessageWindow();
            sendMessageWindow.Title = string.Format("{0}({1}: {2})",
                ResourcesHelper.GetValue("SendMessageString", "发送消息"),
                ResourcesHelper.GetValue("ToString", "给"),
                ResourcesHelper.GetValue("AllString", "全部"));
            if (sendMessageWindow.ShowDialog() == true)
            {
                var message = sendMessageWindow.MessageTextBox.Text;
                var buffer = Encoding.UTF8.GetBytes(message);
                var dst = new byte[buffer.Length + 4];
                var flag = Constants.SEND_MESSAGE_TO_ALL_FLAG;
                SetFlag(dst, flag, 0);
                Buffer.BlockCopy(buffer, 0, dst, 4, buffer.Length);
                buffer = null;
                foreach (var client in _items)
                {
                    client.Channel.Send(dst);
                }
                dst = null;
            }
        }

        private bool CanSendMessageToAll()
        {
            return null != _items && _items.Count > 0;
        }

        private void Rename()
        {
            if (null != _selectedClient)
            {
                var client = _selectedClient;
                var renameWindow = new RenameWindow();
                renameWindow.RenameGroupBox.Header = client.IPAddress;
                renameWindow.HostNameTextBox.Text = client.HostName;
                if (renameWindow.ShowDialog() == true)
                {
                    var hostname = renameWindow.HostNameTextBox.Text;
                    var buffer = Encoding.UTF8.GetBytes(hostname);
                    var dst = new byte[buffer.Length + 4];
                    var flag = Constants.MODIFY_HOSTNAME_FLAG;
                    SetFlag(dst, flag, 0);
                    Buffer.BlockCopy(buffer, 0, dst, 4, buffer.Length);
                    buffer = null;
                    _tcpServer.SendTo(client.Channel, dst);
                    client.HostName = hostname;
                    dst = null;
                }
            }
        }

        private bool CanRename()
        {
            return null != _selectedClient;
        }

        #endregion
    }
}
