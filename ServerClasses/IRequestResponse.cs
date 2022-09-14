using NetModelsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerDatabase;

namespace ServerClasses
{
    public interface IRequestResponse : IClientModel
    {
        public event Action<RequestType, string> OnSuccess;
        public event Action<RequestType, string> OnFailure;

        public void ResponseSuccess(RequestType type, string message);
        public void ResponseFailure(RequestType type, string message);
        public void ResponseChats(IEnumerable<Chat> chats);
        public void ResponseUsers(IEnumerable<User> users);
        public void ResponseMessagePage(int from, IEnumerable<Message> messages);
    }
}
