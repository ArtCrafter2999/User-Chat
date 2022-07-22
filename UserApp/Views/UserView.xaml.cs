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
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl, INotifyPropertyChanged
    {
        public ICommand Click { get; set; } 
        public UserModel UserModel { get; set; }
        public UserView(UserModel userModel)
        {
            InitializeComponent();
            DataContext = this;
            UserModel = userModel;
            Click = new RelayCommand((o) => { });
            OnPropertyChanged(nameof(UserModel));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
