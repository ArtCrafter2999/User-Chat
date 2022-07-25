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
    }
}
