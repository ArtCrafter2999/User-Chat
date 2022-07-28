using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetModelsLibrary.Models;
using UserApp.Views;

namespace UserApp.Models
{
    public class ChatModel : INotifyProperyChangedBase
    {
        private NetModelsLibrary.Models.ChatModel _chat;
        public int Id => _chat.Id;
        public string Title => _chat.Title;
        public bool IsTrueTitle => _chat.IsTrueTitle;
        public int UnreadedMessageCount { get => _chat.Unreaded; set { _chat.Unreaded = value; } }
        public List<MessageModel> Messages { get; set; } = new List<MessageModel>();
        public int Loaded = 0;
        public bool IsEnd = false;
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
        public DateTime LastTime
        {
            get
            {
                if (LastMessage != null && _chat.DateOfChange != null) return LastMessage.SendTime < _chat.DateOfChange.Value ? LastMessage.SendTime : _chat.DateOfChange.Value;
                if (_chat.DateOfChange != null) return _chat.DateOfChange.Value;
                if (LastMessage != null) return LastMessage.SendTime;
                return _chat.CreationDate;
            }
        }
        public MessageModel? LastMessage
        { 
            get
            {
                if (Messages != null && Messages.Count > 0) return Messages.Last();
                return _chat.LastMessage != null ? new MessageModel(_chat.LastMessage) : null;
            }
            set
            {
                if (value != null)
                {
                    if (Messages != null && Messages.Count > 0) Messages.Add(value);
                    else _chat.LastMessage = value._message;
                    OnPropertyChanged(nameof(LastMessage));
                }
            }
        }
        public ChatModel(NetModelsLibrary.Models.ChatModel model)
        {
            _chat = model;
        }
        public ChatView? ChatView { get; set; }
    }
}
