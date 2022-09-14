using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerClasses
{
    public interface IRequestHandler : IRequestTypeMethods, IClientModel, IRequestTypeMethodsBind
    {
    }
}
