using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetModelsLibrary.Models;

namespace UserApp.Models
{
    public class ChatModel
    {
        private NetModelsLibrary.Models.ChatModel _chat;
        public int Id => _chat.Id;
        public string Title => _chat.Title;
        public int UnridedMessageCount => throw new NotImplementedException();
        public List<UserModel> Users
        {
            get
            {
                var users = new List<UserModel>();
                foreach (var user in _chat.Users)
                {
                    users.Add(new UserModel(user));
                }
                return users;
            }
        }
        public DateTime LastTime => _chat.LastMessage != null ? _chat.LastMessage.SendTime : _chat.CreationDate;
        public MessageModel LastMessage => new MessageModel(_chat.LastMessage);
        public ChatModel(NetModelsLibrary.Models.ChatModel model)
        {
            _chat = model;
        }
        public List<MessageModel> LoadMessages()
        {
            throw new NotImplementedException();
        }
    }
}
