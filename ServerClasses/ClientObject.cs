using Microsoft.EntityFrameworkCore;
using NetModelsLibrary;
using ServerDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerClasses
{
    public class ClientObject : IClient
    {
        public INetwork Network { get; set; }
        public IRequestResponse Respondent { get; set; }
        public IRequestListener Listener { get; set; }
        public IClientsNotifyer Notifyer { get; set; }
        public IRequestHandler Handler { get; set; }
        public IClient Client { get; set; }

        private static Dictionary<int, ClientObject> UsersOnline { get; set; } = new Dictionary<int, ClientObject>();
        public const int PageSize = 20;

        public TcpClient TcpClient { get; set; }
        public NetworkStream NetworkStream => TcpClient.GetStream();

        private User? _user;

        public event Action<User> OnDisconected;

        public User User
        {
            get
            {
                if (_user != null) return _user;
                else return new User()
                {
                    Id = -1,
                    Login = "unknown",
                    Name = "unknown",
                    PasswordMD5 = "unknown"
                };
            }
            set { _user = value; }
        }

        public ClientObject(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        public void Disconect()
        {
            UserOffline();
            if (NetworkStream != null) NetworkStream.Close();
            if (TcpClient != null) TcpClient.Close();
            OnDisconected?.Invoke(User);
        }

        public void UserOnline()
        {
            try
            {
                UsersOnline.Add(User.Id, this);
                Notifyer.UserChangeStatus();
            }
            catch { }
        }
        public void UserOffline()
        {
            try
            {
                UsersOnline.Remove(User.Id);
                using (var db = new ServerDbContext())
                {
                    User.LastOnline = DateTime.Now;
                    db.SaveChanges();
                }
                Notifyer.UserChangeStatus();
            }
            catch{}
        }

        public ClientObject GetOnlineUser(int userId)
        {
            return UsersOnline[userId];
        }

        public bool IsUserOnline(int userId)
        {
            return UsersOnline.ContainsKey(userId);
        }
    }
}
