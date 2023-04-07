using NetModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApp.ViewModels
{
    public abstract class NetworkResoulter
    {
        public event Action<ResultModel>? Success;
        public event Action<ResultModel>? Failure;
        public void Invoke(ResultModel model)
        {
            if (model.Success)
            {
                Success?.Invoke(model);
            }
            else
            {
                Failure?.Invoke(model);
            }
        }
    }
}
