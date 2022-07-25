using UserApp.Models;
using UserApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commands;
using System.Windows.Input;
using System.Windows.Controls;

namespace UserApp.ViewModels
{
    public class ChatCreationViewModel : INotifyProperyChangedBase
    {
        public bool Visibility { get => _visibility; set { _visibility = value; OnPropertyChanged(nameof(Visibility)); } }
        private bool _visibility = false;

        public AddUserViewModel AddUserView { get; set; }

        public MainWindow MainWindow => MainWindow.instance;

        public NetModelsLibrary.Models.ChatCreationModel ChatCreationModel { get; set; } = new NetModelsLibrary.Models.ChatCreationModel();

        public List<UserModel> RelatedUsers { get; set; } = new List<UserModel>();
        public List<UserModel> AddedUsers { get; set; } = new List<UserModel>();

        public event Action ChatCreated;

        public ICommand Create => new RelayCommand(o =>
        {
            ChatCreationModel.Users = new List<NetModelsLibrary.Models.IdModel>();
            foreach (var user in AddedUsers)
            {
                ChatCreationModel.Users.Add(new NetModelsLibrary.Models.IdModel(user.Id));
            }
            Connection.Network.WriteRequest(NetModelsLibrary.RequestType.CreateChat);
            Connection.Network.WriteObject(ChatCreationModel);
            if (Connection.Network.ReadObject<NetModelsLibrary.Models.ResoultModel>().Success) ChatCreated?.Invoke();
            
            AddedUsers.Clear();
            ChatCreationModel = new NetModelsLibrary.Models.ChatCreationModel();
            AddUserView.Visibility = false;
            AddUserView.Users.Clear();
            AddUserView.SearchModel.SearchString = "";
            AddUserView.timer.Stop();
            AddUserView.OnPropertyChanged(nameof(AddUserView.Visibility));
            AddUserView.OnPropertyChanged(nameof(AddUserView.SearchModel));
        }, o => AddedUsers.Count > 0);

        public ICommand AddUser => new RelayCommand(o =>
        {
            if (AddUserView.Visibility == false)
            {
                AddUserView.Visibility = true;
                AddUserView.timer.Start();
            }
        });
        public ICommand Cancel => new RelayCommand(o =>
        {
            AddUserView.Visibility = false;
            Visibility = false;
            AddUserView.timer.Stop();
            MainWindow.OverlayGrid.HideAll();
        });

        public ChatCreationViewModel()
        {
            AddUserView = new AddUserViewModel();
            ChatCreated += () => { MainWindow.instance.ChatController.SelectedChatModel = null; MainWindow.UpdateChatView(); };
        }
        public void UpdateUsersView()
        {
            RelatedUsers.Clear();
            foreach (var chat in MainWindow.ChatController.ChatModels)
            {
                foreach (var user in chat.Users)
                {
                    if (!AddedUsers.Contains(user) && !RelatedUsers.Contains(user) && MainWindow.ChatController.SelfUser != user) RelatedUsers.Add(user);
                }
            }
            RelatedUsers.Sort((a, b) => string.Compare(a.Name, b.Name));

            MainWindow.AddUsersInChat.Children.Clear();
            MainWindow.AddedUsersInChat.Children.Clear();

            MainWindow.AddUsersInChat.Children.Add(new Button()
            {
                FontSize = 17,
                Content = "+ Додати нового",
                Background = System.Windows.Media.Brushes.White,
                Foreground = System.Windows.Media.Brushes.SteelBlue,
                Command = AddUser
            });
            foreach (var user in RelatedUsers)
            {
                MainWindow.AddUsersInChat.Children.Add(new UserView(user) 
                {
                    Click = new RelayCommand(o =>
                    {
                        if (!AddedUsers.Contains(user)) AddedUsers.Add(user);
                        UpdateUsersView();
                    })
               });
            }
            foreach (var user in AddedUsers)
            {
                MainWindow.AddedUsersInChat.Children.Add(new UserView(user)
                {
                    Click = new RelayCommand(o =>
                    {
                        AddedUsers.Remove(user);
                        UpdateUsersView();
                    })
                });
            }
        }
        //private void NotAlreadyContained(UserModel user)
        //{

        //}
    }
}
