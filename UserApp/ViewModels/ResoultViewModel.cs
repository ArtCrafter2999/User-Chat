﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetModelsLibrary.Models;

namespace UserApp.ViewModels
{
    public class ResoultViewModel : INotifyProperyChangedBase
    {
        public bool FailureVisibility { get => _failureVisibility; set { _failureVisibility = value; OnPropertyChanged(nameof(FailureVisibility)); } }
        private bool _failureVisibility = false;
        public string FailureText { get => _failureText; set { _failureText = value; OnPropertyChanged(nameof(FailureText)); } }
        private string _failureText = "Ти не повинен бачити цей текст";
        public bool SuccessVisibility { get => _successVisibility; set { _successVisibility = value; OnPropertyChanged(nameof(SuccessVisibility)); } }
        private bool _successVisibility = false;
        public string SuccessText { get => _successText; set { _successText = value; OnPropertyChanged(nameof(SuccessText)); } }
        private string _successText = "Ти не повинен бачити цей текст";

        public void Hide()
        {
            FailureVisibility = false;
            SuccessVisibility = false;
        }
        public void AddBind(NetworkResoulter resoulter)
        {
            resoulter.Success += Success;
            resoulter.Failure += Failure;
        }
        private void Failure(ResultModel result)
        {
            Hide();
            FailureVisibility = true;
            FailureText = result.Message;
        }
        private void Success(NetModelsLibrary.Models.ResultModel result)
        {
            Hide();
            SuccessVisibility = true;
            SuccessText = result.Message;
        }
    }
}
