using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApp.Views;
using UserApp.Models;


namespace UserApp.Controllers
{
    public class MainWindowController : INotifyProperyChangedBase
    {
        public ObservableCollection<ChatModel> ChatModels { get; set; } = new ObservableCollection<ChatModel>();
        public ChatModel? SelectedChatModel { get => _selectedChatModel; set { _selectedChatModel = value; OnPropertyChanged(nameof(SelectedChatModel)); MainWindow.OnPropertyChanged(nameof(MainWindow.IsSelected)); } }
        private ChatModel? _selectedChatModel = null;

        

        public MainWindow MainWindow { get; set; }
        public MainWindowController(MainWindow window)
        {
            MainWindow = window;
        }
    }
}
