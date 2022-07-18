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
        private NetModelsLibrary.Models.UserModel _user;
        public UserModel(NetModelsLibrary.Models.UserModel model)
        {
            _user = model;
        }
    }
}
