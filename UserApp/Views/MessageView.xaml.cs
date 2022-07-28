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
    public partial class MessageView : UserControl, INotifyPropertyChanged
    {
        public MessageModel MessageModel { get; set; }
        public double HalfWidth => Width / 4;
        public bool IsMyMessage => MainWindow.instance.ChatController.SelfUser.Id == MessageModel.User.Id;
        public bool IsOthersMessage => !IsMyMessage;

        public MessageView(MessageModel messageModel)
        {
            InitializeComponent();
            DataContext = this;
            MessageModel = messageModel;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
