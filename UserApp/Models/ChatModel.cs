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
        public List<MessageModel> Messages { get; set; } = new List<MessageModel>();
        public int PagesLoaded = 0;
        public bool IsEndPage = false;
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
        public DateTime LastTime => _lastMessage != null ? _lastMessage.SendTime : _chat.CreationDate;
        public MessageModel? LastMessage {
            get 
            {
                return _chat.LastMessage != null ? _lastMessage : null;
            } 
            set
            {
                _lastMessage = value;
            }
        }
        private MessageModel? _lastMessage;
        public ChatModel(NetModelsLibrary.Models.ChatModel model)
        {
            _chat = model;
            _lastMessage = _chat.LastMessage != null ? new MessageModel(_chat.LastMessage) : null;
        }
        public List<MessageModel> LoadMessages()
        {
            throw new NotImplementedException();
        }
    }
}
