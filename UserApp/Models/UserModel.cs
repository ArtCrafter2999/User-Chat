using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetModelsLibrary.Models;

namespace UserApp.Models
{
    public class UserModel
    {
        private UserStatusModel _user;
        public int Id => _user.Id;
        public string Name => _user.Name;
        public string Login => _user.Login;
        public string LoginString => $"{_user.Login}({_user.Id})";
        public bool IsOnline => _user.IsOnline;
        public DateTime? LastOnline => _user.LastOnline;
        public UserModel(UserStatusModel model)
        {
            _user = model;
        }
        public static bool operator ==(UserModel left, UserModel right)
        {
            return left.Id == right.Id;
        }
        public static bool operator !=(UserModel left, UserModel right)
        {
            return left.Id != right.Id;
        }
    }
}
