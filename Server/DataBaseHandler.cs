using Microsoft.EntityFrameworkCore;
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
    class OperationFailureExeption : Exception
    {
        public RequestType? RequestType { get; set; }
        public OperationFailureExeption(RequestType type, string message) : base(message) { RequestType = type; }
        public OperationFailureExeption(string message) : base(message) { }
        public OperationFailureExeption() : base("не вдалося виконати цю операцію") {}
    }
    public class DataBaseHandler
    {
        public static List<int> UsersOnline { get; set; } = new List<int>();
        public static Dictionary<int, Network> NetworkOfId { get; set; } = new Dictionary<int, Network>();
        private User? _user;
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
        }
        public Network network => Client.network;
        public ClientObject Client { get; set; }
        public NotifyUserController Notifyer { get; set; }
        public DataBaseHandler(ClientObject client)
        {
            Client = client;
            Notifyer = new NotifyUserController(client, this);
        }

        public void UserOnline()
        {
            UsersOnline.Add(User.Id);
            NetworkOfId.Add(User.Id, network);
        }
        public void UserOffline()
        {
            UsersOnline.Remove(User.Id);
            NetworkOfId.Remove(User.Id);
            using (var db = new ServerDbContext())
            {
                User.LastOnline = DateTime.Now;
                db.SaveChanges();
            }
        }

        public void Registration(UserCreationModel model)
        {
            Console.WriteLine($"'{model.Login}' registration");
            using (var db = new ServerDbContext())
            {
                var user = db.Users.Count() > 0 ? db.Users.Where((u) => u.Login == model.Login).FirstOrDefault() : null;
                if (user == null)
                {
                    var newuser = new User() { Login = model.Login, PasswordMD5 = model.PasswordMD5, Name = model.Name };
                    db.Users.Add(newuser);
                    db.SaveChanges();
                    _user = newuser;
                }
                else
                {
                    throw new OperationFailureExeption($"Login '{model.Login}' is alrady exist, request rejected");
                }
            }
            network.WriteObject(new ResoultModel(RequestType.Registration, true, "You registered successfuly"));
            Console.WriteLine($"{User.Login}({User.Id}) registration successfuly");
            UserOnline();
        }
        public void Auth(AuthModel model)
        {
            Console.WriteLine($"'{model.Login}' authorization");
            using (var db = new ServerDbContext())
            {
                var user = db.Users.Count() > 0 ? db.Users.Where((u) => u.Login == model.Login).FirstOrDefault() : null;
                if (user != null && user.PasswordMD5 == model.PasswordMD5)
                {
                    if (!UsersOnline.Contains(user.Id))
                        _user = user;
                    else throw new OperationFailureExeption($"У ваш аккаунт вже виконан вхід з іншого вікна");

                }
                else
                {
                    throw new OperationFailureExeption($"Incorrect login or password (а конкретно {(user == null ? "логін" : "пароль")})");
                }
            }
            network.WriteObject(new ResoultModel(RequestType.Auth, true, "You authorized successfuly"));
            Console.WriteLine($"{User.Login}({User.Id}) authorization successfuly");
            UserOnline();
        }

        public void SendMessage(MessageModel model)
        {
            Console.WriteLine($"{User.Login}({User.Id}) request send message to chat ({model.ChatId})");
            try
            {
                using (var db = new ServerDbContext())
                {
                    if (User.Id == -1) throw new OperationFailureExeption();
                    var message = new Message() { Text = model.Text, UserId = User.Id, ChatId = model.ChatId, SendTime = model.SendTime};
                    db.Messages.Add(message);
                    db.SaveChanges();

                    var list = new List<ServerDatabase.File>();
                    foreach (var fileinfo in model.Files)
                    {
                        var file = new ServerDatabase.File() { Message = message, Name = fileinfo.Name, Size = fileinfo.DataSize, Format = fileinfo.Format };
                        db.Files.Add(file);
                        list.Add(file);
                    }
                    db.SaveChanges();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].ServerPath =
                            network.ReadFile($"Files\\{list[i].Id}_{model.Files[i].Name}.{model.Files[i].Format}",
                            model.Files[i]
                        );
                    }
                    db.SaveChanges();
                    model.Id = message.Id;
                    network.WriteObject(model);
                }
            }
            catch (OperationFailureExeption)
            {
                throw new OperationFailureExeption("Unable to send message from unregistered user");
            }
        }

        public void GetAllChats()
        {
            Console.WriteLine($"{User.Login}({User.Id}) Request for get all chats with him");
            AllChatsModel res;
            using (var db = new ServerDbContext())
            {
                var chats = from Chat c in db.Chats.Include(c => c.Users).Include(c => c.Messages).Include(c => c.UserChatRelatives)
                            where c.Users.Contains(User)
                            select c;
                res = new AllChatsModel();
                res.User = new UserStatusModel()
                {
                    Id = User.Id,
                    Login = User.Login,
                    Name = User.Name,
                    IsOnline = true,
                    LastOnline = null
                };
                res.Chats = new List<ChatModel>();
                foreach (var chat in chats)
                {

                    var userStatusModels = new List<UserStatusModel>();
                    if (chat.Users != null)
                    {
                        foreach (var user in chat.Users)
                        {
                            userStatusModels.Add(new UserStatusModel()
                            {
                                Id = user.Id,
                                Name = user.Name,
                                Login = user.Login,
                                IsOnline = IsUserOnline(user),
                                LastOnline = user.LastOnline
                            });
                        }
                    }

                    MessageModel? messageModel = null;
                    try
                    {
                        var message = chat.Messages.OrderByDescending(c => c.SendTime).First();
                        if (message != null)
                        {
                            User user = db.Users.Find(message.UserId);
                            if (user != null)
                            messageModel = new MessageModel() // повідомлення, частина якого буде відображатися під чатом, як останне
                            {
                                Id = message.Id,
                                ChatId = chat.Id,
                                User = new UserStatusModel()
                                {
                                    Id = user.Id,
                                    IsOnline = IsUserOnline(user),
                                    LastOnline = user.LastOnline,
                                    Login = user.Login,
                                    Name = user.Name
                                },
                                SendTime = message.SendTime,
                                Text = message.Text ?? "Відправлено Файл"
                            };
                        }
                    }
                    catch (Exception) { }

                    res.Chats.Add(new ChatModel()
                    {
                        Id = chat.Id,
                        Title = chat.Title ?? GenerateChatName(chat),
                        Users = userStatusModels,
                        LastMessage = messageModel,
                        CreationDate = chat.CreationDate,
                        Unreaded = chat.UserChatRelatives.Find(ucr => ucr.User.Id == User.Id).Unreaded
                    });
                }
                network.WriteObject(res);
                Console.WriteLine($"Returned {res.Chats.Count} chats with {res.User.Login}({res.User.Id})");
            }
        }

        public string GenerateChatName(Chat chat)
        {
            var users = from User user in chat.Users
                        where user.Id != User.Id
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
        public bool IsUserOnline(User user)
        {
            return UsersOnline.Contains(user.Id);
        }

        public void CreateChat(ChatCreationModel model)
        {
            Console.WriteLine($"{User.Login}({User.Id}) Request for ChatCreation");
            using (var db = new ServerDbContext())
            {
                var chat = new Chat();
                chat.Title = model.Title;
                chat.CreationDate = DateTime.Now;
                chat.Messages = new List<Message>();
                chat.Users = new List<User>();

                db.Chats.Add(chat);
                db.SaveChanges();
                model.Users.Add(new IdModel(User.Id));

                var userStatusModels = new List<UserStatusModel>();
                foreach (var UserIdModel in model.Users)
                {
                    var user = db.Users.Find(UserIdModel.Id);
                    if (user != null)
                    {
                        chat.Users.Add(user);
                        userStatusModels.Add(new UserStatusModel()
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Login = user.Login,
                            IsOnline = IsUserOnline(user),
                            LastOnline = user.LastOnline
                        });
                        db.UserChatRelatives.Add(new UserChatRelative()
                        {
                            User = user,
                            Chat = chat,
                            Unreaded = 0
                        });
                    }
                }
                db.SaveChanges();

                Notifyer.ChatCreated(new ChatModel()
                {
                    Id = chat.Id,
                    CreationDate = chat.CreationDate,
                    Title = chat.Title ?? GenerateChatName(chat),
                    Users = userStatusModels,
                    Unreaded = 0,
                    LastMessage = null
                });
                Console.WriteLine($"{User.Login}({User.Id})'s Chat created for {model.Users.Count} users");
            }
            network.WriteObject(new ResoultModel(RequestType.CreateChat, true, "You successfuly created a new chat"));
        }

        public void SearchUsers(SearchModel model)
        {
            Console.WriteLine($"{User.Login}({User.Id}) Request for serch users with searchString '{model.SearchString}'");
            var allusers = new List<User>();
            using (var db = new ServerDbContext())
            {
                int id;
                bool IsIdParsed = int.TryParse(model.SearchString, out id);
                if (IsIdParsed)
                {
                    var IdentityId = db.Users.Find(id);
                    if (IdentityId != null && !allusers.Contains(IdentityId))
                        allusers.Add(IdentityId);

                }
                var IdentityLogin = (from User u in db.Users
                                     where u.Login.ToLower() == model.SearchString.ToLower() && !allusers.Contains(u)
                                     select u).FirstOrDefault();
                if (IdentityLogin != null)
                    allusers.Add(IdentityLogin);
                var IdentityName = (from User u in db.Users
                                    where u.Name.ToLower() == model.SearchString.ToLower() && !allusers.Contains(u)
                                    select u).FirstOrDefault();
                if (IdentityName != null)
                    allusers.Add(IdentityName);

                if (IsIdParsed)
                {
                    var SimilarId = from User u in db.Users
                                    where u.Id.ToString().Contains(model.SearchString) && !allusers.Contains(u)
                                    select u;
                    allusers.AddRange(SimilarId);
                }
                var SimilarLogin = from User u in db.Users
                                   where u.Login.ToLower().Contains(model.SearchString.ToLower()) && !allusers.Contains(u)
                                   select u;
                allusers.AddRange(SimilarLogin);
                var SimilarUsername = from User u in db.Users
                                      where u.Name.ToLower().Contains(model.SearchString.ToLower()) && !allusers.Contains(u)
                                      select u;
                allusers.AddRange(SimilarUsername);

                _user = db.Users.Find(User.Id);
                allusers.Remove(_user);

                var res = new AllUsersModel();
                foreach (var user in allusers)
                {
                    res.Users.Add(new UserStatusModel()
                    {
                        Id = user.Id,
                        Login = user.Login,
                        Name = user.Name,
                        IsOnline = IsUserOnline(user),
                        LastOnline = user.LastOnline,
                    });
                }
                network.WriteObject(res);
                Console.WriteLine($"For {User.Login}({User.Id})'s search request '{model.SearchString}' Founded {allusers.Count} users");
            }
        }

        public const int PageSize = 20; 
        public void GetPageOfMessages(GetMessagesInfoModel model)
        {
            Console.WriteLine($"{User.Login}({User.Id}) Request to get messages in range {model.PageNumber * PageSize}-{model.PageNumber+1 * PageSize} from chat ({model.ChatId})");
            using (var db = new ServerDbContext())
            {
                var page = new MessagesPageModel()
                {
                    PageNumber = model.PageNumber + 1
                };
                var messages = db.Messages
                    .Include(m => m.Chat)
                    .Include(m => m.User)
                    .Where(m => m.ChatId == model.ChatId)
                    .OrderByDescending(m => m.SendTime)
                    .Skip(model.PageNumber * PageSize)
                    .Take(PageSize);
                if (messages.Count() < PageSize) page.IsEnd = true;
                foreach (var message in messages)
                {
                    page.Messages.Add(new MessageModel()
                    {
                        ChatId = model.ChatId,
                        Id = message.Id,
                        SendTime = message.SendTime,
                        Text = message.Text,
                        User = new UserStatusModel
                        {
                            Id = message.UserId,
                            IsOnline = IsUserOnline(message.User),
                            LastOnline = message.User.LastOnline,
                            Login = message.User.Login,
                            Name = message.User.Name
                        }
                    });
                }
                network.WriteObject(page);
                Console.WriteLine($"{User.Login}({User.Id}) received {page.Messages.Count} messages");
            }
        }

    }
}
