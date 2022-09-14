using NetModelsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UserApp.Controllers;

namespace UserApp
{
    public static class Connection
    {
        public static int Port { get; set; }
        public static IPAddress Ip { get; set; }
        public static TcpClient Client { get; set; }
        public static NetworkStream Stream { get; set; }
        public static INetwork Network { get; set; }
        public static bool IsConnected { get; set; } = false;
        public static CancellationTokenSource NetworkCancelation;
        public static void Disconect()
        {
            NetworkCancelation.Cancel();
            Stream?.Close();
            Client?.Close();
        }
    }
}
