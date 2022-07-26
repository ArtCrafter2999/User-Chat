using NetModelsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetModelsLibrary.Models;
using UserApp.Views;
using System.Windows.Threading;
using System.Windows;
#pragma warning disable SYSLIB0006

namespace UserApp.Controllers
{
    public class ReceiveNotifyController
    {
        public MainWindow MainWindow => MainWindow.instance;
        public ChatController ChatController => MainWindow.ChatController;

        private Thread? Thread = null;
        public void Stop()
        {
            Thread?.Abort();
        }
        public void Start()
        {
            Thread = new Thread(new ThreadStart(Process));
            Thread.Start();
        }
        public void Process()
        {
            try
            {
                if (Connection.Stream != null)
                {
                    while (true)
                    {
                        var Info = Connection.Network.ReadObject<NotifyInfoModel>();
                        switch (Info.Type)
                        {
                            case NotifyType.ChatCreated:
                                ChatCreated(Connection.Network.ReadObject<ChatModel>());
                                break;
                            case NotifyType.MessageSended:
                                MessageSended(Connection.Network.ReadObject<MessageModel>());
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public void ChatCreated(ChatModel model)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                ChatController.ChatModels.Add(new Models.ChatModel(model));
                MainWindow.SortChats();
            });
        }
        public void MessageSended(MessageModel model)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                if (MainWindow.ChatView.IsSelected && model.ChatId == ChatController.SelectedChatModel.Id)
                {
                    Connection.Network.WriteRequest(RequestType.MarkReaded);
                    Connection.Network.WriteObject(new IdModel(model.ChatId));
                    MainWindow.ChatView.MessageDown(new Models.MessageModel(model));
                    ChatController.SelectedChatModel.ChatView.Unreaded = 0;
                }
                else
                {
                    var chat = ChatController.ChatModels.Find(c => c.Id == model.ChatId);
                    chat.ChatView.Unreaded++;
                }
            });
        }
    }
}
