using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserApp.Models;
using UserApp.Controllers;
using Commands;

namespace UserApp.Views
{
    /// <summary>
    /// Interaction logic for ChatViewModel.xaml
    /// </summary>
    public partial class ChatViewModel : UserControl, INotifyPropertyChanged
    {
        public ICommand Click { get; set; } 
        public ChatModel ChatModel { get; set; }
        public ChatViewModel(ChatModel chatModel, MainWindow window)
        {
            InitializeComponent();
            DataContext = this;
            ChatModel = chatModel;
            Click = new RelayCommand((o) => {window.ChatController.SelectedChatModel = ChatModel;}, (o) => window.ChatController.SelectedChatModel != ChatModel);
            OnPropertyChanged(nameof(ChatModel));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
