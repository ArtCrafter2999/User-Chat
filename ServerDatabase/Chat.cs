using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabase
{
    public class Chat
    {
        public int Id { get; set; }
        public string? Name{ get; set; }

        public virtual List<User> Users { get; set; }
        public virtual List<Message> Messages { get; set; }
    }
}
