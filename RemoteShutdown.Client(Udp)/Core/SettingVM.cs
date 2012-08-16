using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteShutdown.Core;
using RemoteShutdown.Utilities;

namespace RemoteShutdown.Client.Core
{
    public class SettingVM : EntityObject
    {
        #region 变量

        static SettingVM _instance = new SettingVM();

        const string SUB_NAME = "Software\\RemoteShutdown";

        const string BOOT_NAME = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        #endregion

        #region 属性

        public static SettingVM Instance { get { return _instance; } }

        public bool First
        {
            get { return null == RWReg.GetValue(SUB_NAME, "Udp_Client_First", null); }
            set { RWReg.SetValue(SUB_NAME, "Udp_Client_First", "Is not first run."); }
        }

        public bool Boot
        {
            get 
            {
                var boot = (string)RWReg.GetValue(BOOT_NAME, "RemoteShutdown_Udp_Client", string.Empty);
                return !string.IsNullOrEmpty(boot) &&
                    string.Compare(System.Windows.Forms.Application.ExecutablePath,
                        boot, true) == 0;
            }
            set
            {
                if (value)
                {
                    RWReg.SetValue(BOOT_NAME, "RemoteShutdown_Udp_Client", System.Windows.Forms.Application.ExecutablePath);
                }
                else
                {
                    RWReg.RemoveKey(BOOT_NAME, "RemoteShutdown_Udp_Client");
                }
                RaisePropertyChanged("Boot");
            }
        }

        /// <summary>
        /// 是否接受服务器端控制
        /// </summary>
        public bool AllowControl
        {
            get { return Converter.ToInt(RWReg.GetValue(SUB_NAME, "Udp_Client_AllowControl", 1)) != 0; }
            set { RWReg.SetValue(SUB_NAME, "Udp_Client_AllowControl", value ? 1 : 0); }
        }

        /// <summary>
        /// 是否接收广播消息
        /// </summary>
        public bool AllowBroadcast
        {
            get { return Converter.ToInt(RWReg.GetValue(SUB_NAME, "Udp_Client_AllowBroadcast", 1)) != 0; }
            set { RWReg.SetValue(SUB_NAME, "Udp_Client_AllowBroadcast", value ? 1 : 0); }
        }

        #endregion

        #region 构造方法

        private SettingVM() { }

        #endregion
    }
}
