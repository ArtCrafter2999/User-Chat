using ServerDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerClasses
{
    public interface IClient : IClientModel
    {

        public event Action<User> OnDisconected;

        IClient GetOnlineUser(int userId);
        bool IsUserOnline(int userId);
        public void Disconect();
        public void UserOnline();
        public void UserOffline();

        public User User { get; set; }
    }
}
