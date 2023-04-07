using Microsoft.EntityFrameworkCore;
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
    public class ClientsNotifyer : IClientsNotifyer
    {
        public IClient Client { get; set; }
        public IRequestResponse Respondent { get; set; }
        public IRequestListener Listener { get; set; }
        public IClientsNotifyer Notifier { get; set; }
        public IRequestHandler Handler { get; set; }
        public INetwork Network { get; set; }

        public void ChatChanged(Chat model, List<User> added, List<User> removed, List<User> notChanged)
        {
            MessageModel? messageModel = null;
            try
            {
                var message = model.Messages.OrderByDescending(c => c.SendTime).FirstOrDefault();
                if (message != null)
                {
                    var users = new List<User>();
                    users.AddRange(removed);
                    users.AddRange(notChanged);
                    User? user = users.First(u => u.Id == message.UserId);
                    if (user != null)
                        messageModel = new MessageModel(message, Client.IsUserOnline(user.Id)); // повідомлення, частина якого буде відображатися під чатом, як останне
                }
            }
            catch (Exception) { }

            var userStatusModels = new List<UserStatusModel>();
            foreach (var user in notChanged)
            {
                userStatusModels.Add(new UserStatusModel(user, Client.IsUserOnline(user.Id)));
            }
            foreach (var user in added)
            {
                userStatusModels.Add(new UserStatusModel(user, Client.IsUserOnline(user.Id)));
            }

            var reschat = new ChatModel()
            {
                Id = model.Id,
                CreationDate = model.CreationDate,
                DateOfChange = model.DateOfChange,
                Title = model.Title ?? GenerateChatName(model),
                IsTrueTitle = model.Title != null,
                Users = userStatusModels,
                Unreaded = 0,
                LastMessage = messageModel
            };

            foreach (var user in added)
            {
                if (Client.IsUserOnline(user.Id))
                {
                    var net = Client.GetOnlineUser(user.Id).Network;
                    net.WriteNotify(NotifyType.ChatCreated);
                    net.WriteObject(reschat);
                }
            }
            foreach (var user in removed)
            {
                if (Client.IsUserOnline(user.Id))
                {
                    var net = Client.GetOnlineUser(user.Id).Network;
                    net.WriteNotify(NotifyType.ChatDeleted);
                    net.WriteObject(new IdModel(reschat.Id));
                }
            }
            foreach (var user in notChanged)
            {
                if (Client.IsUserOnline(user.Id))
                {
                    var net = Client.GetOnlineUser(user.Id).Network;
                    net.WriteNotify(NotifyType.ChatChanged);
                    net.WriteObject(reschat);
                }
            }
        }
        public string GenerateChatName(Chat chat)
        {
            var users = from User user in chat.Users
                        where user.Id != Client.User.Id
                        select user;
            var names = new List<string>();
            foreach (var user in users)
            {
                names.Add(user.Name);
            }
            return string.Join(", ", names);
        }
        public string GenerateChatName(ChatModel chat, UserStatusModel userexclusive)
        {
            var users = from UserStatusModel user in chat.Users
                        where user.Id != userexclusive.Id
                        select user;
            var names = new List<string>();
            foreach (var user in users)
            {
                names.Add(user.Name);
            }
            return string.Join(", ", names);
        }

        public void ChatCreated(Chat model)
        {
            var userStatusModels = new List<UserStatusModel>();
            foreach (var user in model.Users)
            {
                userStatusModels.Add(new UserStatusModel(user, Client.IsUserOnline(user.Id)));

            }
            var chat = new ChatModel()
            {
                Id = model.Id,
                CreationDate = model.CreationDate,
                Title = model.Title ?? GenerateChatName(model),
                IsTrueTitle = model.Title != null,
                Users = userStatusModels,
                Unreaded = 0,
                LastMessage = null
            };


            foreach (var user in userStatusModels)
            {
                if (user.Id != Client.User.Id && Client.IsUserOnline(user.Id))
                {
                    var userNetwork = Client.GetOnlineUser(user.Id).Network;
                    userNetwork.WriteNotify(NotifyType.ChatCreated);
                    chat.Title = model.Title ?? GenerateChatName(chat, user);
                    userNetwork.WriteObject(chat);
                }
            }

        }

        public void ChatDeleted(int ChatId, List<User> users)
        {
            foreach (var user in users)
            {
                if (Client.IsUserOnline(user.Id))
                {
                    var net = Client.GetOnlineUser(user.Id).Network;
                    net.WriteNotify(NotifyType.ChatDeleted);
                    net.WriteObject(new IdModel(ChatId));
                }
            }
        }

        public void MessageSended(Message message, Chat chat)
        {
            var model = new MessageModel(message, Client.IsUserOnline(message.User.Id));
            foreach (var user in chat.Users)
            {
                if (user.Id != Client.User.Id)
                {
                    AddUnreaded(chat.Id, user.Id);
                    if (Client.IsUserOnline(user.Id))
                    {
                        var userNetwork = Client.GetOnlineUser(user.Id).Network;
                        userNetwork.WriteNotify(NotifyType.MessageSended);
                        userNetwork.WriteObject(model);
                    }
                }
                else
                {
                    Network.WriteNotify(NotifyType.MessageSended);
                    Network.WriteObject(model);
                }
            }
        }

        public List<User> GetRelativeUsers()
        {
            List<User> res = new List<User>();
            using (var db = new ServerDbContext())
            {
                var chats = from Chat c in db.Chats.Include(c => c.Users)
                            where c.Users.Contains(Client.User)
                            select c;
                foreach (var chat in chats)
                {
                    foreach (var user in chat.Users)
                    {
                        if (user.Id != Client.User.Id && !res.Contains(user))
                        {
                            res.Add(user);
                        }
                    }
                }
            }
            return res;
        }

        public void UserChangeStatus()
        {
            var rel = GetRelativeUsers();
            foreach (var user in rel)
            {
                if (Client.IsUserOnline(user.Id))
                {
                    var net = Client.GetOnlineUser(user.Id).Network;
                    net.WriteNotify(NotifyType.UserChangeStatus);
                    net.WriteObject(new UserStatusModel(Client.User, Client.IsUserOnline(Client.User.Id)));
                }
            }
        }

        public static void AddUnreaded(int chatId, int userId)
        {
            using (var db = new ServerDbContext())
            {
                var rel = (from UserChatRelative ucr in (from Chat c in db.Chats.Include(c => c.UserChatRelatives)
                                                         where c.Id == chatId
                                                         select c).First().UserChatRelatives
                           where ucr.UserId == userId
                           select ucr).First();
                rel.Unreaded++;
                db.SaveChanges();
            }
        }
    }
}
