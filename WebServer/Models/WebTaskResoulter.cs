using NetModelsLibrary;
using NetModelsLibrary.Models;
using ServerClasses;
using ServerDatabase;

namespace WebServer.Models
{
    public class WebTaskResulter : IRequestResponse
    {
        public IClient Client { get; set; }
        public INetwork Network { get => Client.Network; set { Client.Network = value; } }
        public IRequestResponse Respondent { get => Client.Respondent; set { Client.Respondent = value; } }
        public IRequestListener Listener { get => Client.Listener; set { Client.Listener = value; } }
        public IClientsNotifyer Notifyer { get => Client.Notifyer; set { Client.Notifyer = value; } }
        public IRequestHandler Handler { get => Client.Handler; set { Client.Handler = value; } }

        public TaskCompletionSource<object>? TaskRes => (Client as WebClientObject).TaskRes;

        public event Action<RequestType, string> OnSuccess;
        public event Action<RequestType, string> OnFailure;

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

        public void ResponseChats(IEnumerable<Chat> chats)
        {
            AllChatsModel res = new AllChatsModel();
            res.User = new UserStatusModel()
            {
                Id = Client.User.Id,
                Login = Client.User.Login,
                Name = Client.User.Name,
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
                        userStatusModels.Add(new UserStatusModel(user, Client.IsUserOnline(user.Id)));
                    }
                }

                MessageModel? messageModel = null;
                try
                {
                    var message = chat.Messages.OrderByDescending(c => c.SendTime).First();
                    if (message != null)
                    {
                        User user = chat.Users.First(o => o.Id == message.UserId);
                        if (user != null)
                            messageModel = new MessageModel(message, Client.IsUserOnline(user.Id)); // повідомлення, частина якого буде відображатися під чатом, як останне
                    }
                }
                catch (Exception) { }

                res.Chats.Add(new ChatModel()
                {
                    Id = chat.Id,
                    Title = chat.Title ?? GenerateChatName(chat),
                    IsTrueTitle = chat.Title != null,
                    Users = userStatusModels,
                    LastMessage = messageModel,
                    CreationDate = chat.CreationDate,
                    DateOfChange = chat.DateOfChange,
                    Unreaded = chat.UserChatRelatives.Find(ucr => ucr.User.Id == Client.User.Id).Unreaded
                }); ;
            }
            TaskRes.SetResult(res);
        }

        public void ResponseFailure(RequestType type, string message)
        {
            OnFailure?.Invoke(type, message);
            TaskRes.SetResult(new ResultModel(type, false, message));
        }
        public void ResponseSuccess(RequestType type, string message)
        {
            OnSuccess?.Invoke(type, message);
            TaskRes.SetResult(new ResultModel(type, true, message));
        }

        public void ResponseMessagePage(int from, IEnumerable<Message> messages)
        {
            var page = new MessagesPageModel()
            {
                From = from
            };
            var count = messages.Count();
            if (count < ClientObject.PageSize) page.IsEnd = true;
            page.To = from + count;
            foreach (var message in messages)
            {
                page.Messages.Add(new MessageModel(message, Client.IsUserOnline(message.User.Id)));
            }
            TaskRes.SetResult(page);
        }

        public void ResponseUsers(IEnumerable<User> users)
        {
            var res = new AllUsersModel();
            foreach (var user in users)
            {
                res.Users.Add(new UserStatusModel(user, Client.IsUserOnline(user.Id)));
            }
            TaskRes.SetResult(res);
        }
    }
}
