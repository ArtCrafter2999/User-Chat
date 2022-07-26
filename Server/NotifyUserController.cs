using NetModelsLibrary;
using NetModelsLibrary.Models;
using ServerDatabase;
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
                string? origTitle = model.Title;
                if (user.Id != Handler.User.Id && IsUserOnline(user.Id))
                {
                    var userNetwork = DataBaseHandler.NetworkOfId[user.Id];
                    userNetwork.WriteNotify(NotifyType.ChatCreated);
                    model.Title = origTitle ?? Handler.GenerateChatName(model, user);
                    userNetwork.WriteObject(model);
                }
            }
        }
        private bool IsUserOnline(int userId)
        {
            return DataBaseHandler.UsersOnline.Contains(userId);
        }

        public void MessageSended(MessageModel message, Chat chat)
        {
            foreach (var user in chat.Users)
            {
                Handler.AddUnreaded(chat.Id, user.Id);
                if (user.Id != Handler.User.Id && IsUserOnline(user.Id))
                {
                    var userNetwork = DataBaseHandler.NetworkOfId[user.Id];
                    userNetwork.WriteNotify(NotifyType.MessageSended);
                    userNetwork.WriteObject(message);
                }
            }
        }
    }
}
