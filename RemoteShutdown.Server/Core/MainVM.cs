using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using RemoteShutdown.Core;
using RemoteShutdown.Net;
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

        private ClientModel _selectedClient;

        #endregion

        #region 属性

        public static MainVM Instance { get { return _instance; } }

        public ObservableCollection<ClientModel> Items { get { return _items; } }

        public DelegateCommand<object> PowerCommand { get { return _powerCommand; } }

        public DelegateCommand<object> PowerAllCommand { get { return _powerAllCommand; } }

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
                    case 1001:  // 连接
                        Connected(channel, buffer);
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
                    }));
                }
            }
            catch (Exception exception)
            {
                _logger.Debug("[Connected] Exception : {0}", exception.Message);
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
            var powerText = powerType == 0 ? "注销" : (powerType == 1 ? "关机" : "重启");
            var buffer = BitConverter.GetBytes(powerType);
            if (MessageBox.Show(Application.Current.MainWindow,
                string.Format("是否确定执行全部{0}命令?", powerText),
                "询问",
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

        private bool CanPowerAll()
        {
            return null != _items && _items.Count > 0;
        }

        #endregion
    }
}
