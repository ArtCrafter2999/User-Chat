using NetModelsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetModelsLibrary.Models;
using UserApp.Views;
using System.Windows.Threading;
using System.Windows;
#pragma warning disable SYSLIB0006

namespace UserApp.Controllers
{
    public class ReceiveNotifyController
    {
        public MainWindow MainWindow => MainWindow.instance;
        public ChatController ChatController => MainWindow.ChatController;

        TaskCompletionSource<string>? tcs;

        public void Stop()
        {
            if (tcs != null)
                tcs.SetException(new Exception());
        }
        public void Start()
        {
            tcs = new TaskCompletionSource<string>();
            Task.Factory.StartNew(Process);
        }
        public void Process()
        {
            try
            {
                if (Connection.Stream != null)
                {
                    while (true)
                    {
                        var Info = Connection.Network.ReadObject<NotifyInfoModel>();
                        switch (Info.Type)
                        {
                            case NotifyType.ChatCreated:
                                ChatCreated(Connection.Network.ReadObject<ChatModel>());
                                break;
                            case NotifyType.MessageSended:
                                MessageSended(Connection.Network.ReadObject<MessageModel>());
                                break;
                            case NotifyType.UserChangeStatus:
                                UserChangeStatus(Connection.Network.ReadObject<UserStatusModel>());
                                break;
                            case NotifyType.ChatChanged:
                                ChatChanged(Connection.Network.ReadObject<ChatModel>());
                                break;
                            case NotifyType.ChatDeleted:
                                ChatDeleted(Connection.Network.ReadObject<IdModel>());
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public void ChatCreated(ChatModel model)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                ChatController.ChatModels.Add(new Models.ChatModel(model));
                MainWindow.SortChats();
            });
        }
        public void ChatDeleted(IdModel model)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                var chat = ChatController.ChatModels.Find(c => c.Id == model.Id);
                if (chat != null)
                    ChatController.ChatModels.Remove(chat);
                else MessageBox.Show("chat == null, ReceiveNotifyContoller.cs:line 78", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (ChatController.SelectedChatModel != null && model.Id == ChatController.SelectedChatModel.Id)
                {
                    ChatController.SelectedChatModel = null;
                }
                MainWindow.SortChats();
            });
        }
        public void ChatChanged(ChatModel model)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                var view = ChatController.ChatModels[ChatController.ChatModels.FindIndex(c => c.Id == model.Id)].ChatView;
                ChatController.ChatModels[ChatController.ChatModels.FindIndex(c => c.Id == model.Id)] = new Models.ChatModel(model);
                if (view != null)
                {
                    view.ChatModel = new Models.ChatModel(model);
                    view.OnPropertyChanged(nameof(view.ChatModel));
                    view.OnPropertyChanged(nameof(view.LastMessageView));
                    view.OnPropertyChanged(nameof(view.IsOnline));
                }
                if (ChatController.SelectedChatModel != null && model.Id == ChatController.SelectedChatModel.Id)
                {
                    ChatController.SelectedChatModel = null;
                }
                MainWindow.SortChats();
            });
        }
        public void MessageSended(MessageModel model)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                if (MainWindow.ChatView.IsSelected && model.ChatId == ChatController.SelectedChatModel.Id)
                {
                    Connection.Network.WriteRequest(RequestType.MarkReaded);
                    Connection.Network.WriteObject(new IdModel(model.ChatId));
                    var message = new Models.MessageModel(model);
                    MainWindow.ChatView.MessageDown(message);
                    ChatController.SelectedChatModel.LastMessage = message;
                    if (ChatController.SelectedChatModel.ChatView != null)
                    {
                        ChatController.SelectedChatModel.ChatView.OnPropertyChanged(nameof(ChatController.SelectedChatModel.ChatView.LastMessageView));
                        ChatController.SelectedChatModel.ChatView.Unreaded = 0;
                    }
                }
                else
                {
                    var chat = ChatController.ChatModels.Find(c => c.Id == model.ChatId);
                    if (chat?.ChatView != null)
                    {
                        chat.LastMessage = new Models.MessageModel(model);
                        chat.ChatView.OnPropertyChanged(nameof(chat.ChatView.LastMessageView));
                        chat.ChatView.Unreaded++;
                    }
                }
            });
        }
        public void UserChangeStatus(UserStatusModel model)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                foreach (var chat in ChatController.ChatModels)
                {
                    var user = chat.Users.Find(u => u.Id == model.Id);
                    if (user != null)
                    {
                        var view = user.UserView;
                        user.SetUser(model);
                        if (view != null)
                        {
                            user.UserView = view;
                            view.UserModel = user;
                            view?.OnPropertyChanged(nameof(view.UserModel));
                        }
                        if (chat.Users.Count == 2)
                        {
                            chat.Users[chat.Users.FindIndex(u => u.Id != MainWindow.instance.ChatController.SelfUser.Id)].IsOnline = model.IsOnline;
                            chat.ChatView?.OnPropertyChanged(nameof(chat.ChatView.IsOnline));
                        }
                    }
                }
            });
        }
    }
}
