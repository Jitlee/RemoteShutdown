using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace RemoteShutdown.Utilities
{
    public static class Common
    {
        public static readonly int ServerPort = 12535;
        public static readonly int ClientPort = 12536;
        public static readonly int Port = 12537;
        public static readonly int UdpBufferSize = 64;

        /// <summary>
        /// 服务器断开重连时间间隔
        /// </summary>
        public static readonly int AutoConnectInterval = 10000;

        public static string GetLocalIPAddress()
        {
            var ipAddress = string.Empty;

            System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            foreach (System.Net.IPAddress ip in host.AddressList)
            {

                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {

                    ipAddress = ip.ToString();

                    break;

                }

            }

            return ipAddress;
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr hMenu, int nIndex, int dwNewLong);

        const int GWL_STYLE = -16;
        const int WS_MAXIMIZEBOX = 0x00010000;
        const int WS_MINIMIZEBOX = 0x00020000;

        public static void DisableMinmize(IntPtr hWnd)
        {
            SetWindowLong(hWnd, GWL_STYLE, GetWindowLong(hWnd, GWL_STYLE) & ~WS_MINIMIZEBOX);
        }
    }
}
