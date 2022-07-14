using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabase
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string ServerPath { get; set; }
        public string Format { get; set; }
        public long Size { get; set; }

        public int MessageId { get; set; }
        public virtual Message Message { get; set; }
    }
}
