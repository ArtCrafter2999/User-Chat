using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabase
{
    public class Message
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public DateTime SendTime { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int ChatId { get; set; }
        public virtual Chat Chat { get; set; }

        public virtual List<File> Files { get; set; }
    }
}
