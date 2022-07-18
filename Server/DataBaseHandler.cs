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
        public User User { get { if (_user != null) return User; else throw new OperationFailureExeption("Unable to send message from unregistered user"); } }
        public Network network => Client.network;
        public ClientObject Client;
        public DataBaseHandler(ClientObject client)
        {
            Client = client;
        }

        public string? LoginFromUserId(int Id)
        {
            if (Id == -1) return "unknown";
            User? user;
            using (var db = new ServerDbContext())
            {
                user = db.Users.Find(Id);
            }
            return user != null ? user.Login : "unknown";
        }
        /// <summary>
        /// Повертає або екземпляр User або новий User з параметрами unknown
        /// </summary>
        /// <returns>Повертає або екземпляр User або новий User з параметрами unknown</returns>
        public User SafeUserGet()
        {
            if (_user != null) return _user;
            else return new User() { Id = -1, Login = "unknown", Name = "unknown", PasswordMD5 = "unknown" };
        }
        public void Registration(AuthModel model)
        {
            Console.WriteLine($"'{model.Login}' registration");
            using (var db = new ServerDbContext())
            {
                var user = db.Users.Count() > 0? db.Users.Where((u) => u.Login == model.Login).FirstOrDefault() : null;
                if (user == null)
                {
                    network.WriteObject(new ResoultModel() { RequestType = RequestType.Registration, Success = true, Message = "You picked unique login" });
                    UserModel userData = network.ReadObject<UserModel>();
                    _user = new User() { Login = model.Login, PasswordMD5 = model.PasswordMD5, Name = userData.Name };
                    db.Users.Add(_user);
                    var chat = new Chat() { Users = new List<User>()};
                    db.Chats.Add(chat);
                    db.SaveChanges();
                    var user1 = db.Users.Find(1);
                    if (user1 != null && user1 != _user)
                    {
                        chat.Users.Add(user1);
                        chat.Users.Add(_user);
                    }
                    db.SaveChanges();
                    

                }
                else
                {
                    throw new OperationFailureExeption($"Login '{model.Login}' is alrady exist, request rejected");
                }
            }
            network.WriteObject(new ResoultModel() { RequestType = RequestType.Registration, Success = true, Message = "You registered successfuly" });
            Console.WriteLine($"'{model.Login}' registration successfuly");
        }
        public void Auth(AuthModel model)
        {
            Console.WriteLine($"'{model.Login}' authorization");
            using (var db = new ServerDbContext())
            {
                var user = db.Users.Count() > 0 ? db.Users.Where((u) => u.Login == model.Login).FirstOrDefault() : null;
                if (user != null && user.PasswordMD5 == model.PasswordMD5)
                {
                    _user = user;
                }
                else
                {
                    throw new OperationFailureExeption($"Incorrect login or password (а конкретно {(user == null? "логін" : "пароль")})");
                }
            }
            network.WriteObject(new ResoultModel() { RequestType = RequestType.Auth, Success = true, Message = "You authorized successfuly" });
            Console.WriteLine($"'{model.Login}' authorization successfuly");
        }
        public void Message(MessageModel model)
        {
            Console.WriteLine($"'{User.Name}'({User.Id}) > send message > chatid: {model.ChatId};\n content:\n\n {model.Text}");
            try
            {
                using (var db = new ServerDbContext())
                {
                    var message = new Message() { ChatId = model.ChatId, Text = model.Text, User = User };
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
                throw new OperationFailureExeption("Unable to send message from unregistered user");
            }
        }
    }
}
