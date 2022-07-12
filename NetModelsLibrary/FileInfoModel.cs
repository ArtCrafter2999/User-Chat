using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModelsLibrary
{
    public class FileInfoModel
    {
        public string Title { get; set; }
        public string Format { get; set; }
        public long DataSize { get; set; }
        public int PackageCount { get; set; }
    }
}
