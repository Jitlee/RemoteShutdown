using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RemoteShutdown.Net;

namespace RemoteShutdown.Core
{
    public class ClientModel : EntityObject
    {
        #region 变量

        private string _hostName;

        private string _ipAddress;

        [NonSerialized]
        private IChannel _channel;

        #endregion

        #region 属性

        public string HostName { get { return _hostName; } set { _hostName = value; RaisePropertyChanged("HostName"); } }

        public string IPAddress { get { return _ipAddress; } set { _ipAddress = value; RaisePropertyChanged("IPAddress"); } }

        [XmlIgnore]
        public IChannel Channel { get { return _channel; } set { _channel = value; } }

        #endregion

        #region 构造方法

        public ClientModel() { }

        public ClientModel(IChannel channel) { _channel = channel; }

        #endregion

        #region 重载方法

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var client = obj as ClientModel;
            if (null != client
                && null != client._channel
                && null != _channel)
            {
                return client._channel.Equals(_channel);
            }
            return base.Equals(obj);
        }

        #endregion
    }
}
