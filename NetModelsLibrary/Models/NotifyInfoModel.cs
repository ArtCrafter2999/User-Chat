using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModelsLibrary.Models
{
    public class NotifyInfoModel
    {
        public NotifyType Type { get; set; }
        public NotifyInfoModel(NotifyType type)
        {
            Type = type;
        }
        public NotifyInfoModel() { }
    }
}
