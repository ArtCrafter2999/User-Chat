using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModelsLibrary.Models
{
    public class ChatModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<UserModel> Users { get; set; }
        public MessageModel LastMessage { get; set; }
    }
}
