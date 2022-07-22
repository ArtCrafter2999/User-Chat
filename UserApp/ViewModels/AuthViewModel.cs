using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Commands;
using NetModelsLibrary.Models;
using NetModelsLibrary;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UserApp.ViewModels
{
    public class AuthViewModel : NetworkResoulter, INotifyPropertyChanged
    {
        public string Login { get; set; }
        public string Username { get; set; }

        public bool Visibility { get => _visibility; set { _visibility = value; OnPropertyChanged(nameof(Visibility)); } }
        private bool _visibility = false;

        public ICommand Authorize => new RelayCommand(o =>
        {
            if (Connection.IsConnected)
            {
                Connection.Network.WriteRequest(RequestType.Auth);
                Connection.Network.WriteObject(new AuthModel() { Login = Login, PasswordMD5 = CreateMD5(((PasswordBox)o).Password) });
                Invoke(Connection.Network.ReadObject<ResoultModel>());
            }
            else
            {
                Invoke(new ResoultModel(false, "Немає з'єднання з сервером"));
            }
        });
        public ICommand Register => new RelayCommand(o =>
        {
            if (Connection.IsConnected)
            {
                Connection.Network.WriteRequest(RequestType.Registration);
                Connection.Network.WriteObject(new UserCreationModel() { Name = Username, Login = Login, PasswordMD5 = CreateMD5(((PasswordBox)o).Password) });

                ResoultModel? resoult = Connection.Network.ReadObject<ResoultModel>();
                Invoke(resoult);
            }
            else
            {
                Invoke(new ResoultModel(false, "Немає з'єднання з сервером"));
            }
        });

        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
