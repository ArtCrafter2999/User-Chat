using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApp.Views;
using UserApp.Models;
using System.Windows;

namespace UserApp.Controllers
{
    public class MainWindowController : INotifyProperyChangedBase
    {
        public List<ChatModel> ChatModels { get; set; } = new List<ChatModel>();
        public UserModel SelfUser { get; set; }
        public ChatModel? SelectedChatModel { get => _selectedChatModel; set { _selectedChatModel = value; OnPropertyChanged(nameof(SelectedChatModel)); MainWindow.OnPropertyChanged(nameof(MainWindow.IsSelected)); } }
        private ChatModel? _selectedChatModel = null;
        public ViewModels.OverlayGrid OverlayGrid => MainWindow.OverlayGrid;

        public MainWindow MainWindow => MainWindow.instance;
        public MainWindowController()
        {}

        public void LoadChats()
        {
            Connection.Network.WriteRequest(NetModelsLibrary.RequestType.GetAllChats);
            var allchats = Connection.Network.ReadObject<NetModelsLibrary.Models.AllChatsModel>();
            SelfUser = new UserModel(allchats.User);
            ChatModels.Clear();
            foreach (var NetChatModel in allchats.Chats)
            {
                ChatModels.Add(new ChatModel(NetChatModel));
            }
        }
    }
}
