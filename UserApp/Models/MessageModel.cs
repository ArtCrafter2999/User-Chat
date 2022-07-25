using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetModelsLibrary.Models;
using UserApp.Views;

namespace UserApp.Models
{
    public class MessageModel
    {
        private NetModelsLibrary.Models.MessageModel _message;
        public int? Id => _message.Id;
        public int ChatId => _message.ChatId;
        public ChatModel? Chat => MainWindow.instance.ChatController.ChatModels.Find(c => c.Id == ChatId);
        public DateTime SendTime => _message.SendTime;
        public string Text => _message.Text ?? "";
        public List<FileInfoModel> Files => _message.Files;
        public UserModel User => new UserModel(_message.User);
        public MessageModel(NetModelsLibrary.Models.MessageModel model)
        {
            _message = model;
        }
        
    }
}
