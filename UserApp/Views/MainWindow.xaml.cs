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
using UserApp.ViewModels;

namespace UserApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static MainWindow instance { get; set; }

        public MainWindowController ChatController { get; set; }
        public bool IsSelected => ChatController.SelectedChatModel != null;

        public OverlayGrid OverlayGrid { get; set; }

        public ICommand CreateChat => new RelayCommand(o =>
        {
            OverlayGrid.Visibility = true;
            OverlayGrid.ChatCreationView.Visibility = true;
            OverlayGrid.ChatCreationView.UpdateUsersView();
            OverlayGrid.ChatCreationView.ChatCreated += OverlayGrid.HideAll;
        });

        public MainWindow()
        {
            try
            {
                instance = this;

                InitializeComponent();
                DataContext = this;

                ChatController = new MainWindowController();
                OverlayGrid = new OverlayGrid(/*this*/);
                OverlayGrid.AuthView.Success += _ => UpdateChatView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateChatView()
        {
            ChatController.LoadChats();
            ChatsStack.Children.Clear();
            List<ChatModel> copy = ChatController.ChatModels.ToList();
            copy.Sort((a, b) => a.LastTime < b.LastTime ? 1 : -1);
            foreach (var model in copy)
            {
                ChatsStack.Children.Add(new ChatView(model));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
