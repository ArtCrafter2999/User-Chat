using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModelsLibrary.Models
{
    public class MessageModel
    {
        public int ChatId { get; set; }
        public string Message { get; set; }
        public List<FileInfoModel> Files { get; set; }
    }
}
