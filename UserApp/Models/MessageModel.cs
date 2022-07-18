using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetModelsLibrary.Models;

namespace UserApp.Models
{
    public class MessageModel
    {
        private NetModelsLibrary.Models.MessageModel _message;
        public MessageModel(NetModelsLibrary.Models.MessageModel model)
        {
            _message = model;
        }
    }
}
