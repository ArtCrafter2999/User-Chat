using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ServerDatabase
{
    public class UserChatRelative
    {
        [Key]
        public int UserId { get; set; }
        [Key]
        public int ChatId { get; set; } 
        public virtual Chat Chat { get; set; } 
        public virtual User User { get; set; }
        public int Unreaded { get; set; }
    }
}
