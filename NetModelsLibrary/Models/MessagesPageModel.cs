using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModelsLibrary.Models
{
    public class MessagesPageModel
    {
        public List<MessageModel> Messages { get; set; } = new List<MessageModel>();
        public int PageNumber { get; set; }
        public bool IsEnd { get; set; } = false;
    }
}
