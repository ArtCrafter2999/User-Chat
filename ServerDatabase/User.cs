using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabase
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Login { get; set; }
        public string PasswordMD5 { get; set; }

        public DateTime? LastOnline { get; set; }

        public virtual List<Chat> Chats { get; set; }
        public virtual List<Message> Messages { get; set; }
    } 
}
