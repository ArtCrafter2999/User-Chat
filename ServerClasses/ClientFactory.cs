using NetModelsLibrary;
using System.Net.Sockets;

namespace ServerClasses
{
    public class ClientFactory : IClientModel
    {
        public INetwork Network { get; set; }
        public IClient Client { get; set; }
        public IRequestResponse Respondent { get; set; }
        public IRequestListener Listener { get; set; }
        public IClientsNotifyer Notifier { get; set; }
        public IRequestHandler Handler { get; set; }

        public IClient MakeClient()
        {
            var list = new List<IClientModel>();
            list.Add(Client ?? throw new Exception("Client не має значення"));
            list.Add(Respondent ?? throw new Exception("Respondent не має значення"));
            list.Add(Listener ?? throw new Exception("Listener не має значення"));
            list.Add(Notifier ?? throw new Exception("Notifyer не має значення"));
            list.Add(Handler ?? throw new Exception("Handler не має значення"));
            Handler.Bind(Listener);


            foreach (var clientModel in list)
            {
                clientModel.Network = Network;
                clientModel.Client = Client;
                clientModel.Respondent = Respondent;
                clientModel.Listener = Listener;
                clientModel.Notifier = Notifier;
                clientModel.Handler = Handler;
            }
            return Client;
        }
    }
}