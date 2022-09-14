using NetModelsLibrary;
using NetModelsLibrary.Models;
using ServerDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerClasses
{
    public interface IClientsNotifyer : IClientModel
    {
        public void ChatCreated(Chat model);
        public void MessageSended(Message message, Chat chat);
        public void UserChangeStatus();
        public void ChatChanged(Chat model, List<User> added, List<User> removed, List<User> notChanged);
        public void ChatDeleted(int ChatId, List<User> users);
    }
}
