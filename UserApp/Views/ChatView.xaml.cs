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
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl, INotifyPropertyChanged
    {
        public ICommand Click { get; set; } 
        public ChatModel ChatModel { get; set; }
        public string LastMessageView => ChatModel.LastMessage != null? (ChatModel.Users.Count > 2 ? ChatModel.LastMessage.User.Name + ": " + ChatModel.LastMessage.Text : ChatModel.LastMessage.Text) : "";
        public Brush Color 
        { 
            get
            {
                if (MainWindow.instance.ChatController.SelectedChatModel == ChatModel) return Brushes.LightSteelBlue;
                return Brushes.SteelBlue;
            }
        }
        public int Unreaded { get { return ChatModel.UnreadedMessageCount; } set { ChatModel.UnreadedMessageCount = value; OnPropertyChanged(nameof(Unreaded)); OnPropertyChanged(nameof(IsUnreaded)); } }
        public bool IsUnreaded => Unreaded > 0;
        public bool IsOnline => ChatModel.Users.Count == 2 ?
            ChatModel.Users.First(u => u.Id != MainWindow.instance.ChatController.SelfUser.Id).IsOnline : false;
        public ChatView(ChatModel chatModel)
        {
            InitializeComponent();
            DataContext = this;
            ChatModel = chatModel;
            Click = new RelayCommand((o) => 
            {
                if (MainWindow.instance.ChatController.SelectedChatModel != ChatModel)
                    MainWindow.instance.ChatController.SelectedChatModel = ChatModel;
                foreach (ChatView chat in MainWindow.instance.ChatsStack.Children)
                {
                    chat.OnPropertyChanged(nameof(Color));
                }
            });
            OnPropertyChanged(nameof(ChatModel));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
