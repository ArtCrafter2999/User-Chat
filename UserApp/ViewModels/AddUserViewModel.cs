using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApp.Models;
using Commands;
using System.Windows.Threading;

namespace UserApp.ViewModels
{
    public class AddUserViewModel : INotifyProperyChangedBase
    {
        public bool Visibility { get => _visibility; set { _visibility = value; OnPropertyChanged(nameof(Visibility)); } }
        private bool _visibility = false;

        public Views.MainWindow MainWindow => Views.MainWindow.instance;

        public NetModelsLibrary.Models.SearchModel SearchModel { get; set; } = new NetModelsLibrary.Models.SearchModel();

        public List<UserModel> Users { get; set; } = new List<UserModel>();

        public RelayCommand Add => new RelayCommand(o =>
        {
            if (!MainWindow.OverlayGrid.ChatCreationView.AddedUsers.Contains((UserModel)o))
            {
                MainWindow.OverlayGrid.ChatCreationView.AddedUsers.Add((UserModel)o);
                MainWindow.OverlayGrid.ChatCreationView.UpdateUsersView();
            }
            
        });
        public DispatcherTimer timer { get; set; }
        public AddUserViewModel()
        {
            timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 500),
            };
            timer.Tick += (_, __) => { if (SearchModel.SearchString != null && SearchModel.SearchString != "") SearchRequestSend(); };
            
        }
        public void UpdateUsersList()
        {
            MainWindow.FindUsers.Children.Clear();

            foreach (var user in Users)            {
                var view = new Views.UserView(user) { Click = Add };
                view.Button.CommandParameter = user;
                MainWindow.FindUsers.Children.Add(view);
            }
        }
        private string _pastResult = "";
        public void SearchRequestSend()
        {
            if (SearchModel.SearchString != _pastResult)
            {
                _pastResult = SearchModel.SearchString;
                Connection.Network.WriteRequest(NetModelsLibrary.RequestType.SearchUsers);
                Connection.Network.WriteObject(SearchModel);
                var allusers = Connection.Network.ReadObject<NetModelsLibrary.Models.AllUsersModel>();
                MainWindow.Dispatcher.Invoke(() => {
                    Users.Clear();
                    foreach (var user in allusers.Users)
                    {
                        Users.Add(new UserModel(user));
                    }
                    UpdateUsersList();
                });
            }
        }
    }
}
