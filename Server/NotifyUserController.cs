using NetModelsLibrary;
using NetModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class NotifyUserController
    {
        public Network network => Client.network;
        public ClientObject Client { get; set; }
        public DataBaseHandler Handler { get; set; }
        public NotifyUserController(ClientObject client, DataBaseHandler dataBaseHandler)
        {
            Client = client;
            Handler = dataBaseHandler;
        }

        public void ChatCreated(ChatModel model)
        {
            foreach (var user in model.Users)
            {
                if (user.Id != Handler.User.Id && IsUserOnline(user.Id))
                {
                    var userNetwork = DataBaseHandler.NetworkOfId[user.Id];
                    userNetwork.WriteNotify(NotifyType.ChatCreated);
                    userNetwork.WriteObject(model);
                }
            }
        }
        private bool IsUserOnline(int userId)
        {
            return DataBaseHandler.UsersOnline.Contains(userId);
        }
    }
}
