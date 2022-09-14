using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetModelsLibrary;

namespace ServerClasses
{
    public interface IRequestListener : IClientModel, IRequestTypeEvents
    {
        public event Action<RequestType> RequestReceived;

        public void BeginListen();
        public void EndListen();
    }
}
