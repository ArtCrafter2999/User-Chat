using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApp.Views;
using System.Net.Sockets;
using System.Net;
using NetModelsLibrary;
using NetModelsLibrary.Models;
using System.Windows;

namespace UserApp.Controllers
{
    public class ServerConnectionController
    {
        public string Login { get; set; }
        public string Username { get; set; }
        //public MainWindow MainWindow { get; set; }
        //public ServerConnectionController(MainWindow window)
        //{
        //    MainWindow = window;
        //}
        public event Action<ResoultModel> Success;
        public event Action<ResoultModel> Failure;

        public ServerConnectionController()
        {
            //Failure += res => MessageBox.Show(res.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void Authorize(string Password)
        {
            if (Connection.IsConnected)
            {
                Connection.Network.WriteObject(new RequestInfoModel(RequestType.Auth));
                Connection.Network.WriteObject(new AuthModel() { Login = Login, PasswordMD5 = CreateMD5(Password)});
                var resoult = Connection.Network.ReadObject<ResoultModel>();
                if (resoult.Success)
                {
                    Success?.Invoke(resoult);
                }
                else
                {
                    Failure.Invoke(resoult);
                }
            }
            else
            {
                Failure?.Invoke(new ResoultModel() { Message = "Немає з'єднання з сервером" });
            }
        }
        public void Register(string Password)
        {
            if (Connection.IsConnected)
            {
                Connection.Network.WriteObject(new RequestInfoModel(RequestType.Registration));
                Connection.Network.WriteObject(new AuthModel() { Login = Login, PasswordMD5 = CreateMD5(Password) });

                ResoultModel? resoult = Connection.Network.ReadObject<ResoultModel>();
                if (resoult.Success)
                {
                    Connection.Network.WriteObject(new UserModel() { Name = Username, Login = Login });
                    if (resoult.Success)
                    {
                        Success?.Invoke(resoult);
                    }
                    else
                    {
                        Failure.Invoke(resoult);
                    }
                }
                else
                {
                    Failure.Invoke(resoult);
                }
            }
            else
            {
                Failure?.Invoke(new ResoultModel() { Message = "Немає з'єднання з сервером" });
            }
        }
        public void Connect(string ip)
        {
            try
            {
                Connection.Port = 8000;
                Connection.Client = new TcpClient();
                Connection.Client.Connect(IPAddress.Parse(ip), Connection.Port);
                Connection.Stream = Connection.Client.GetStream();
                Connection.Network = new Network(Connection.Stream);
                Connection.IsConnected = true;
                Success?.Invoke(new ResoultModel() { RequestType = null, Message = "Підключення до серверу встановлено"});
            }
            catch (Exception)
            {
                Failure?.Invoke(new ResoultModel() { Message = "Сервер вимкнений або його немає за даною адресою" });
            }
        }

        
        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
