using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerClasses
{
    public interface IRequestTypeMethodsBind
    {
        public void Bind(IRequestTypeEvents events);
    }
}
