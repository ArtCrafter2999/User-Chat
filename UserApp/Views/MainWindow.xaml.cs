using System;
using System.Collections.Generic;
using System.Linq;
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
using UserApp.Controllers;
using UserApp.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Commands;

namespace UserApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindowController ChatController { get; set; }
        public ServerConnectionController ConnectionController { get; set; }
        public bool IsSelected => ChatController.SelectedChatModel != null;

        #region Параметри візуальних плашок поверх grid'a
        public  bool    OverlappedGridVisibility    { get => _overlappedGridVisibility; set { _overlappedGridVisibility = value; OnPropertyChanged(nameof(OverlappedGridVisibility)); } }
        private bool    _overlappedGridVisibility   = true;
        public  bool    AuthVisibility              { get => _authVisibility;       set { _authVisibility       = value; OnPropertyChanged(nameof(AuthVisibility       )); } }
        private bool    _authVisibility             = false;
        public  bool    ConnectionVisibility        { get => _connectionVisibility; set { _connectionVisibility = value; OnPropertyChanged(nameof(ConnectionVisibility )); } }
        private bool    _connectionVisibility       = true;
        public  bool    FailureVisibility           { get => _failureVisibility;    set { _failureVisibility    = value; OnPropertyChanged(nameof(FailureVisibility    )); } }
        private bool    _failureVisibility          = false;
        public  string  FailureText                 { get => _failureText;          set { _failureText          = value; OnPropertyChanged(nameof(FailureText          )); } }
        private string  _failureText                = "Ти не повинен бачити це текст";
        public  bool    SuccessVisibility           { get => _successVisibility;    set { _successVisibility    = value; OnPropertyChanged(nameof(SuccessVisibility    )); } }
        private bool    _successVisibility          = false;
        public  string  SuccessText                 { get => _successText;          set { _successText          = value; OnPropertyChanged(nameof(SuccessText          )); } }
        private string  _successText                = "Ти не повинен бачити це текст";
        #endregion
        public static string Ip { get; set; } 

        public ICommand Authorize => new RelayCommand(o =>
        {
            var box = (PasswordBox)o;
            ConnectionController.Authorize(box.Password);
        });
        public ICommand Register => new RelayCommand(o =>
        {
            var box = (PasswordBox)o;
            ConnectionController.Register(box.Password);
        });
        public ICommand Connect => new RelayCommand(o =>
        {
            var box = (TextBox)o;
            ConnectionController.Connect(box.Text);
        }); // Потрібно вікно підключення до серверу

        private void Failure(NetModelsLibrary.Models.ResoultModel resoult)
        {
            SuccessVisibility = false;
            FailureVisibility = true;
            FailureText = resoult.Message;
        }
        private void Success(NetModelsLibrary.Models.ResoultModel resoult)
        {
            if (resoult.RequestType != null)
            HideAll();
            else 
            {
                AuthVisibility = true;
                ConnectionVisibility = false;
                FailureVisibility = false;
                SuccessVisibility = true;
                SuccessText = resoult.Message;
            }
        }
        private void HideAll()
        {
            OverlappedGridVisibility = false;
            AuthVisibility = false;
            ConnectionVisibility = true;
            FailureVisibility = false;
            SuccessVisibility = false;
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ChatController = new MainWindowController(this);
            ChatController.ChatModels.CollectionChanged += (_1, _2) => UpdateChatView();
            ConnectionController = new ServerConnectionController();
            ConnectionController.Failure += Failure;
            ConnectionController.Success += Success;
            //ChatController.ChatModels.Add(
            //    new ChatModel(new NetModelsLibrary.Models.ChatModel()
            //    {
            //        Title = "цей чат з старим чуваком",
            //        LastMessage = new NetModelsLibrary.Models.MessageModel()
            //        {
            //            Text = "Привіт це моє перше повідомлення. як у тебе справи, я просто хочу перевірити довжину повідомлення",
            //            SendTime = DateTime.Now - new TimeSpan(0, 1, 0)
            //        }
            //    })

            //);
            //ChatController.ChatModels.Add(
            //    new ChatModel(new NetModelsLibrary.Models.ChatModel()
            //    {
            //        Title = "цей чат з тим чуваком",
            //        LastMessage = new NetModelsLibrary.Models.MessageModel()
            //        {
            //            Text = "Привіт це моє перше повідомлення. як у тебе справи, я просто хочу перевірити довжину повідомлення",
            //            SendTime = DateTime.Now
            //        }
            //    })

            //);
        }

        public void UpdateChatView()
        {
            ChatsStack.Children.Clear();
            List<ChatModel> copy = new List<ChatModel>(ChatController.ChatModels);
            copy.Sort((a, b) => a.LastMessageTime < b.LastMessageTime ? 1 : -1);
            foreach (var model in copy)
            {
                ChatsStack.Children.Add(new ChatViewModel(model, this));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
