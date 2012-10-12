using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteShutdown.Core;
using RemoteShutdown.Utilities;
using System.Windows;

namespace RemoteShutdown.Client.Core
{
    public class SettingVM : EntityObject
    {
        #region 变量

        static SettingVM _instance = new SettingVM();

        const string SUB_NAME = "Software\\RemoteShutdown";

        const string BOOT_NAME = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private bool _boot;

        private bool _allowControl = Converter.ToInt(RWReg.GetValue(SUB_NAME, "Tcp_Client_AllowControl", 1)) != 0;

        private bool _allowBroadcast = Converter.ToInt(RWReg.GetValue(SUB_NAME, "Tcp_Client_AllowBroadcast", 1)) != 0;

        private string _serverAddress;

        private string _hostName;

        private readonly DelegateCommand _saveCommand;

        #endregion

        #region 属性

        public static SettingVM Instance { get { return _instance; } }

        public bool First
        {
            get { return null == RWReg.GetValue(SUB_NAME, "Client_First", null); }
            set { RWReg.SetValue(SUB_NAME, "Client_First", "Is not first run."); }
        }

        public bool Boot
        {
            get 
            {
                return _boot;
                //var boot = (string)RWReg.GetValue(BOOT_NAME, "RemoteShutdown_Client", string.Empty);
                //return !string.IsNullOrEmpty(boot) &&
                //    string.Compare(System.Windows.Forms.Application.ExecutablePath,
                //        boot, true) == 0;
            }
            set
            {
                //if (value)
                //{
                //    RWReg.SetValue(BOOT_NAME, "RemoteShutdown_Client", System.Windows.Forms.Application.ExecutablePath);
                //}
                //else
                //{
                //    RWReg.RemoveKey(BOOT_NAME, "RemoteShutdown_Client");
                //}
                _boot = value;
                RaisePropertyChanged("Boot");
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 是否接受服务器端控制
        /// </summary>
        public bool AllowControl
        {
            //get { return Converter.ToInt(RWReg.GetValue(SUB_NAME, "Tcp_Client_AllowControl", 1)) != 0; }
            //set { RWReg.SetValue(SUB_NAME, "Tcp_Client_AllowControl", value ? 1 : 0); }
            get { return _allowControl; }
            set
            {
                _allowControl = value;
                RaisePropertyChanged("AllowControl");
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 是否接收广播消息
        /// </summary>
        public bool AllowBroadcast
        {
            //get { return Converter.ToInt(RWReg.GetValue(SUB_NAME, "Tcp_Client_AllowBroadcast", 1)) != 0; }
            //set { RWReg.SetValue(SUB_NAME, "Tcp_Client_AllowBroadcast", value ? 1 : 0); }
            get { return _allowBroadcast; }
            set
            {
                _allowBroadcast = value;
                RaisePropertyChanged("AllowBroadcast");
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public string ServerAddress
        {
            get { return _serverAddress; }
            set 
            { 
                _serverAddress = value;
                RaisePropertyChanged("ServerAddress");
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 客户端名称
        /// </summary>
        public string HostName
        {
            get
            {
                return _hostName;
            }
            set
            {
                _hostName = value;
                RaisePropertyChanged("HostName");
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand { get { return _saveCommand; } }

        #endregion

        #region 构造方法

        private SettingVM()
        {
            var boot = (string)RWReg.GetValue(BOOT_NAME, "RemoteShutdown_Client", string.Empty);
            _boot = !string.IsNullOrEmpty(boot) &&
                string.Compare(System.Windows.Forms.Application.ExecutablePath,
                    boot, true) == 0;

            _allowControl = Converter.ToInt(RWReg.GetValue(SUB_NAME, "Tcp_Client_AllowControl", 1)) != 0;

            _allowBroadcast = Converter.ToInt(RWReg.GetValue(SUB_NAME, "Tcp_Client_AllowBroadcast", 1)) != 0;

            _serverAddress = (string)RWReg.GetValue(SUB_NAME, "Server_Address", string.Empty);

            _hostName = RWReg.GetValue(SUB_NAME, "Tcp_Client_HostName", string.Empty).ToString();
            if (string.IsNullOrEmpty(_hostName))
            {
                _hostName = Environment.MachineName;
            }

            _saveCommand = new DelegateCommand(Save, CanSave);
        }

        #endregion

        #region 私有方法

        private void Save()
        {
            if (_boot)
            {
                RWReg.SetValue(BOOT_NAME, "RemoteShutdown_Client", System.Windows.Forms.Application.ExecutablePath);
            }
            else
            {
                RWReg.RemoveKey(BOOT_NAME, "RemoteShutdown_Client");
            }


            RWReg.SetValue(SUB_NAME, "Tcp_Client_AllowControl", _allowControl ? 1 : 0);

            RWReg.SetValue(SUB_NAME, "Tcp_Client_AllowBroadcast", _allowBroadcast ? 1 : 0);

            if (HasServerAddressChanged())
            {
                RWReg.SetValue(SUB_NAME, "Server_Address", _serverAddress);

                if (MessageBox.Show("设置已更改，是否立即生效？", "询问", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK) == MessageBoxResult.OK)
                {
                    MainVM.Instance.Connect();
                }
            }

            if (HasHostNameChanged())
            {
                RWReg.SetValue(SUB_NAME, "Tcp_Client_HostName", _hostName);
                MainVM.Instance.Send(Constants.MODIFY_HOSTNAME_FLAG, Encoding.UTF8.GetBytes(_hostName));
            }
        }

        private bool CanSave()
        {
            return HasBootChanged()
                || HasServerAddressChanged()
                || HasAllowControlChanged()
                || HasAllowBroadcastChanged()
                || HasHostNameChanged();
        }

        private bool HasBootChanged()
        {
            var boot = (string)RWReg.GetValue(BOOT_NAME, "RemoteShutdown_Client", string.Empty);
            return (!string.IsNullOrEmpty(boot) &&
                string.Compare(System.Windows.Forms.Application.ExecutablePath,
                    boot, true) == 0) != _boot;
        }

        private bool HasAllowControlChanged()
        {
            var allowControl = Converter.ToInt(RWReg.GetValue(SUB_NAME, "Tcp_Client_AllowControl", 1)) != 0;
            return allowControl != _allowControl;
        }

        private bool HasAllowBroadcastChanged()
        {
            var allowBroadcast = Converter.ToInt(RWReg.GetValue(SUB_NAME, "Tcp_Client_AllowBroadcast", 1)) != 0;
            return _allowBroadcast != allowBroadcast;
        }

        private bool HasServerAddressChanged()
        {
            var serverAddress = (string)RWReg.GetValue(SUB_NAME, "Server_Address", string.Empty);
            return string.Compare(serverAddress, _serverAddress, true) != 0;
        }

        private bool HasHostNameChanged()
        {
            var hostName = RWReg.GetValue(SUB_NAME, "Tcp_Client_HostName", string.Empty).ToString();
            if (string.IsNullOrEmpty(hostName))
            {
                hostName = Environment.MachineName;
            }
            return string.Compare(hostName, _hostName) != 0;
        }

        #endregion
    }
}
