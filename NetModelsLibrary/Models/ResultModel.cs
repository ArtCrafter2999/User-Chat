using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModelsLibrary.Models
{
    public class ResultModel
    {
        public RequestType? RequestType { get; set; } = null;
        public bool Success { get; set; }
        public string? Message { get; set; }

        public ResultModel(){}
        public ResultModel(bool success){ Success = success; }
        public ResultModel(bool success, string message){ Success = success; Message = message; }
        public ResultModel(RequestType? type, bool success, string message){ RequestType = type; Success = success; Message = message; }
    }
}
