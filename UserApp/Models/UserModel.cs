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
        public bool IsOnline { get => _user.IsOnline; set { _user.IsOnline = value; } }
        public DateTime? LastOnline => _user.LastOnline;
        public UserModel(UserStatusModel model)
        {
            SetUser(model);
        }
        public void SetUser(UserStatusModel model)
        {
            _user = model;
        }
        public Views.UserView UserView { get; set; }

        public override bool Equals(object? obj)
        {
            return Id == (obj as UserModel).Id;
        }
    }
}
