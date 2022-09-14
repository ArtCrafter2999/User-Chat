using NetModelsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerClasses
{
    public interface IClientModel
    {
        public INetwork Network { get; set; }
        public IClient Client { get; set; }
        public IRequestResponse Respondent { get; set; }
        public IRequestListener Listener { get; set; }
        public IClientsNotifyer Notifyer { get; set; }
        public IRequestHandler Handler { get; set; }
    }
}
