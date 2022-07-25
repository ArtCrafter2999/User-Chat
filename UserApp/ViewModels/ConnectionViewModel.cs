using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using Commands;
using UserApp.Controllers;
using System.Net.Sockets;
using System.Net;
using NetModelsLibrary.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UserApp.ViewModels
{
    public class ConnectionViewModel : NetworkResoulter, INotifyPropertyChanged
    {
        public bool Visibility { get => _visibility; set { _visibility = value; OnPropertyChanged(nameof(Visibility)); } }
        private bool _visibility = true;

        public ICommand Connect => new RelayCommand(o =>
        {
            try
            {
                var ip = ((TextBox)o).Text == "local" ? GetLocalIPAddress() : IPAddress.Parse(((TextBox)o).Text.Trim());
                Connection.Port = 8000;
                Connection.Client = new TcpClient();
                Connection.Client.Connect(ip, Connection.Port);
                Connection.Stream = Connection.Client.GetStream();
                Connection.Network = new Network(Connection.Stream);
                Connection.IsConnected = true;
                Invoke(new ResoultModel(true, "Підключення до серверу встановлено"));
            }
            catch (Exception)
            {
                Invoke(new ResoultModel(false, "Сервер вимкнений або його немає за даною адресою"));
            }
        });
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
