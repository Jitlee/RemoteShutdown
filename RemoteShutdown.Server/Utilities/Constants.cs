using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Utilities
{
    public static class Constants
    {
        // 注销标记
        /// <summary>
        /// 注销标记
        /// </summary>
        public const int LOG_OFF_FLAG = 0;

        // 关机标记
        /// <summary>
        /// 关机标记
        /// </summary>
        public const int SHUTDOWN_FLAG = 1;

        // 重启标记
        /// <summary>
        /// 重启标记
        /// </summary>
        public const int REBOOT_FLAG = 2;

        // 发送单个消息标记
        /// <summary>
        /// 发送单个消息标记
        /// </summary>
        public const int SEND_MESSAGE_TO_FLAG = 998;

        // 发送给所有人消息标记
        /// <summary>
        /// 发送给所有人消息标记
        /// </summary>
        public const int SEND_MESSAGE_TO_ALL_FLAG = 999;

        // 连接标记
        /// <summary>
        /// 连接标记
        /// </summary>
        public const int CONNECT_FLAG = 1001;

        // 修改终端名标记
        /// <summary>
        /// 修改终端名标记
        /// </summary>
        public const int MODIFY_HOSTNAME_FLAG = 1002;

    }
}
