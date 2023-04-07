using NetModelsLibrary;
using NetModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerClasses
{
    public class RequestListener : IRequestListener
    {
        public INetwork Network { get; set; }
        public IClient Client { get; set; }
        public IRequestResponse Respondent { get; set; }
        public IRequestListener Listener { get; set; }
        public IClientsNotifyer Notifier { get; set; }
        public IRequestHandler Handler { get; set; }


        public event Action<RequestType> RequestReceived;

        public event Action<UserCreationModel> OnRegistration;
        public event Action<MessageModel> OnSendMessage;
        public event Action<AuthModel> OnAuth;
        public event Action OnGetAllChats;
        public event Action<ChatCreationModel> OnCreateChat;
        public event Action<SearchModel> OnSearchUsers;
        public event Action<GetMessagesInfoModel> OnGetPageOfMessages;
        public event Action<IdModel> OnReadUnreaded;
        public event Action<IdModel> OnMarkReaded;
        public event Action<ChatChangeModel> OnChangeChat;
        public event Action<IdModel> OnDeleteChat;

        public RequestListener()
        {
            Listener = this;
            RequestReceived += Invoke;
        }

        private CancellationTokenSource cts;
        public void BeginListen()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            Task.Factory.StartNew(() => Listen(token), token);
        }
        
        public void EndListen()
        {
            cts.Cancel();
        }

        private void Listen(CancellationToken token)
        {
            try
            {
                    while (true)
                    {
                        try
                        {
                            RequestReceived?.Invoke(Network.ReadObject<RequestInfoModel>().Type);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.InnerException);
                            Console.WriteLine(ex.StackTrace);
                        }
                        catch (IOException) { throw; }
                        catch (OperationCanceledException) { throw; }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.InnerException);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
            }
            catch (IOException)
            {
                Client.Disconect();
            }
            catch (OperationCanceledException) { }
            finally
            {
                Client.Disconect();
            }
            
        }

        public void Invoke(RequestType type)
        {
            switch (type)
            {
                case RequestType.Registration:
                    OnRegistration?.Invoke(Network.ReadObject<UserCreationModel>());
                    break;
                case RequestType.SendMessage:
                    OnSendMessage?.Invoke(Network.ReadObject<MessageModel>());
                    break;
                case RequestType.Auth:
                    OnAuth?.Invoke(Network.ReadObject<AuthModel>());
                    break;
                case RequestType.GetAllChats:
                    OnGetAllChats();
                    break;
                case RequestType.CreateChat:
                    OnCreateChat?.Invoke(Network.ReadObject<ChatCreationModel>());
                    break;
                case RequestType.SearchUsers:
                    OnSearchUsers?.Invoke(Network.ReadObject<SearchModel>());
                    break;
                case RequestType.GetPageOfMessages:
                    OnGetPageOfMessages?.Invoke(Network.ReadObject<GetMessagesInfoModel>());
                    break;
                case RequestType.ReadUnreaded:
                    OnReadUnreaded?.Invoke(Network.ReadObject<IdModel>());
                    break;
                case RequestType.MarkReaded:
                    OnMarkReaded?.Invoke(Network.ReadObject<IdModel>());
                    break;
                case RequestType.ChangeChat:
                    OnChangeChat?.Invoke(Network.ReadObject<ChatChangeModel>());
                    break;
                case RequestType.DeleteChat:
                    OnDeleteChat?.Invoke(Network.ReadObject<IdModel>());
                    break;
                default:
                    break;
            }
        }
    }
}
