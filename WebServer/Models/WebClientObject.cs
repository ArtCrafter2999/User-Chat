using NetModelsLibrary;
using ServerClasses;
using ServerDatabase;

namespace WebServer.Models
{
    public class WebClientObject : IClient
    {
        public User User { get; set; }
        public INetwork Network { get; set; }
        public IClient Client { get; set; }
        public IRequestResponse Respondent { get; set; }
        public IRequestListener Listener { get; set; }
        public IClientsNotifyer Notifyer { get; set; }
        public IRequestHandler Handler { get; set; }

        public event Action<User> OnDisconected;
        public TaskCompletionSource<object> TaskRes;

        public T GetActionResult<T, O>(Action<O> action, O param)
        {
            if (TaskRes == null) TaskRes = new TaskCompletionSource<object>();
            Task<object> task = TaskRes.Task;
            Task.Factory.StartNew(
                () => action.Invoke(param)
                );
            return (T)task.Result;
        }

        public void Disconect()
        {
            UserOffline();
            OnDisconected?.Invoke(User);
        }

        public IClient GetOnlineUser(int userId)
        {
            return ClientFactory.UsersOnline[userId];
        }

        public bool IsUserOnline(int userId)
        {
            return ClientFactory.UsersOnline.ContainsKey(userId);
        }

        public void UserOffline()
        {
            try
            {
                ClientFactory.UsersOnline.Remove(User.Id);
                using (var db = new ServerDbContext())
                {
                    User.LastOnline = DateTime.Now;
                    db.SaveChanges();
                }
                Notifyer.UserChangeStatus();
            }
            catch { }
        }

        public void UserOnline()
        {
            try
            {
                ClientFactory.UsersOnline.Add(User.Id, this);
                Notifyer.UserChangeStatus();
            }
            catch { }
        }
    }
}
