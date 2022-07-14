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
        public OperationFailureExeption(RequestType type, string message) : base(message){ RequestType = type; }
        public OperationFailureExeption(string message) : base(message){}
        public OperationFailureExeption() : base(){}
    }
    public class DataBaseHandler
    {
        private static List<User> UsersOnline = new List<User>();
        private User? _user;
        public User User { get { if (_user != null) return User; else throw new OperationFailureExeption("Неможливо виконати дану операцію не авторизованному користувачу");} set { _user = value; } }
        public Network network => Client.network;
        public ClientObject Client;
        public DataBaseHandler(ClientObject client)
        {
            Client = client;
            if (Client.UserId != null)
            {
                using (var db = new ServerDbContext())
                {
                    _user = db.Users.Find(Client.UserId);
                }
                if (_user == null) Client.UserId = null;
            }
        }

        public string? LoginFromUserId (int Id)
        {
            User? user;
            using (var db = new ServerDbContext())
            {
                user = db.Users.Find(Id);
            }
            return user != null ? user.Login : "невідомо";
        }

        public void Registration(AuthModel model)
        {
            Console.WriteLine($"Отриман запит на регістрацію користувача з логіном {model.Login}");
            using (var db = new ServerDbContext())
            {
                var user = db.Users.Count() > 0? db.Users.Where((u) => u.Login == model.Login).FirstOrDefault() : null;
                if (user == null)
                {
                    network.WriteObject(new ResoultModel() { RequestType = RequestType.Registration, Success = true, Message = "Ви обрали унікальний логін" });
                    UserDataModel userData = network.ReadObject<UserDataModel>();
                    db.Users.Add(new User() { Login = model.Login, PasswordMD5 = model.PasswordMD5, Name = userData.Name });
                    db.SaveChanges();
                }
                else
                {
                    throw new OperationFailureExeption($"Логін '{model.Login}' вже існує, запит відхилено");
                }
            }
            network.WriteObject(new ResoultModel() { RequestType = RequestType.Registration, Success = true, Message = "Ви успішно зареєструвалися" });
        }
        public void Auth(AuthModel model)
        {
            Console.WriteLine($"Отриман запит на аутентіфікацю користувача з логіном {model.Login}");
            using (var db = new ServerDbContext())
            {
                var user = db.Users.Count() > 0 ? db.Users.Where((u) => u.Login == model.Login).FirstOrDefault() : null;
                if (user != null && user.PasswordMD5 == model.PasswordMD5)
                {
                    User = user;
                }
                else
                {
                    throw new OperationFailureExeption($"Неправильний логін або пароль (а конкретно {(user == null? "логін" : "пароль")})");
                }
            }
            network.WriteObject(new ResoultModel() { RequestType = RequestType.Auth, Success = true, Message = "Ви успішно авторизовані" });
        }
        public void Message(MessageModel model)
        {
            Console.WriteLine($"Отриман запит на відправлення текстового повідомлення до чату ({model.ChatId}), зміст:\n\n {model.Message}");
            try
            {
                using (var db = new ServerDbContext())
                {
                    var message = new Message() { ChatId = model.ChatId, Text = model.Message, User = User };
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
                            network.ReadFile($"Files\\{model.Files[i].Name}.{model.Files[i].Format}",
                            model.Files[i]
                        );
                    }
                    db.SaveChanges();
                }
            }
            catch (OperationFailureExeption)
            {
                throw new OperationFailureExeption("Неможливо відправити повідомлення від незареєстрованого користувача");
            }
        }
    }
}
