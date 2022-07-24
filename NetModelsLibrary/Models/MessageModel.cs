using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModelsLibrary.Models
{
    public class MessageModel
    {
        public int? Id { get; set; }
        public int ChatId { get; set; }
        public DateTime SendTime { get; set; }
        public string? Text { get; set; }
        public List<FileInfoModel> Files { get; set; } = new List<FileInfoModel>();
        public UserStatusModel User { get; set; }
    }
}
