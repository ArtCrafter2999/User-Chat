using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApp.Controllers;
using UserApp.Views;
using UserApp.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace UserApp.ViewModels
{
    public class ChatMessagesViewModel : INotifyProperyChangedBase
    {
        public MainWindow MainWindow => MainWindow.instance;
        public ChatController Controller => MainWindow.ChatController;
        public ChatModel? selectedChat => Controller.SelectedChatModel;
        public bool IsSelected => Controller.SelectedChatModel != null;

        public ChatMessagesViewModel()
        {
            Controller.ChatChanged += ChatChanged;
            Controller.MessageSended += MessageDown;
            MainWindow.MessageScroll.ScrollChanged += CheckTop;
        }

        private void CheckTop(object sender, ScrollChangedEventArgs e)
        {
            if (selectedChat != null && e.VerticalOffset == 0 && !selectedChat.IsEndPage)
            {
                MessagesUp(LoadMessages());
            }
        }
        public List<MessageModel> LoadMessages()
        {
            var messagespage = Controller.LoadMessages(selectedChat.PagesLoaded);
            selectedChat.PagesLoaded++;
            var newmessages = new List<MessageModel>();
            foreach (var message in messagespage.Messages)
            {
                selectedChat.Messages.Insert(0, new MessageModel(message));
                newmessages.Insert(0, new MessageModel(message));
            }
            selectedChat.IsEndPage = messagespage.IsEnd;
            MainWindow.MessageScroll.ScrollToVerticalOffset(
                MainWindow.MessageScroll.ScrollableHeight /
                selectedChat.PagesLoaded *
                (selectedChat.PagesLoaded - 1)
            );
            return newmessages;
        }

        public void MessagesUp(List<MessageModel> NewMessages)
        {
            int index = 0;
            foreach (var message in NewMessages)
            {
                MainWindow.MessagesStack.Children.Insert(index++, new MessageView(message));
            }
        }

        public void MessagesDown(List<MessageModel> NewMessages)
        {
            foreach (var message in NewMessages)
            {
                MainWindow.MessagesStack.Children.Add(new MessageView(message));
            }
        }

        public void MessageDown(MessageModel message)
        {
            bool scroll = false;
            if (MainWindow.MessageScroll.VerticalOffset == MainWindow.MessageScroll.ScrollableHeight) scroll = true;
            MainWindow.MessagesStack.Children.Add(new MessageView(message));
            if (scroll) 
            { 
                MainWindow.MessageScroll.ScrollToEnd();
                MainWindow.MessageScroll.UpdateLayout(); 
            }
        }
        public void ChatChanged()
        {
            OnPropertyChanged(nameof(IsSelected));
            MainWindow.MessagesStack.Children.Clear();
            if (selectedChat != null)
            {
                if (selectedChat.PagesLoaded == 0)
                {
                    MessagesDown(LoadMessages());
                }
                else
                {
                    MessagesDown(selectedChat.Messages);
                }
            }
            else
            {
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }
}
