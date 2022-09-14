using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetModelsLibrary;
using NetModelsLibrary.Models;

namespace ServerClasses
{
    public  interface IRequestTypeEvents
    {
        /// <summary>
        /// Invokes the appropriate to the type event
        /// </summary>
        /// <param name="type">Type of request</param>
        public void Invoke(RequestType type);
        /// <summary>
        /// Registration request which contain login username and password in MD5
        /// After this server expects that sequence of requests:
        /// Expects UserCreationModel returns ResoultModel
        /// </summary>
        public event Action<UserCreationModel> OnRegistration;
        /// <summary>
        /// Request for send message from the user selected chat
        /// After this server expects MessageModel
        /// </summary>
        public event Action<MessageModel> OnSendMessage;
        /// <summary>
        /// Authorization request which contain login and password in MD5
        /// After this server expects that sequence of requests:
        /// Expects AuthModel returns ResoultModel
        /// </summary>
        public event Action<AuthModel> OnAuth;
        /// <summary>
        /// Request to get all chats which the user is a member
        /// After this server not expects anything, instred it returns AllChatsModel for current user
        /// </summary>
        public event Action OnGetAllChats;
        /// <summary>
        /// Request to create chat
        /// After this server expects that sequence of requests:
        /// Expects ChatCreateModel returns ResoultModel 
        /// </summary>
        public event Action<ChatCreationModel> OnCreateChat;
        /// <summary>
        /// Searching for a new user to add to a new chat
        /// After this server expects that sequence of requests:
        /// Expects SearchModel returns AllUsersModel 
        /// </summary>
        public event Action<SearchModel> OnSearchUsers;
        /// <summary>
        /// Page by page to get messages
        /// After this server expects that sequence of requests:
        /// Expects GetMessagesInfoModel returns MessagesPageModel
        /// </summary>
        public event Action<GetMessagesInfoModel> OnGetPageOfMessages;
        /// <summary>
        /// Get all unreaded messages and mark them readed
        /// After this server expects that sequence of requests:
        /// Expects IdModel witch is chat id and returns MessagesPageModel
        /// </summary>
        public event Action<IdModel> OnReadUnreaded;
        /// <summary>
        /// Mark all unreaded messages as readed
        /// After this server expects that sequence of requests:
        /// Expects IdModel witch is chat id. Returns nothing
        /// </summary>
        public event Action<IdModel> OnMarkReaded;
        /// <summary>
        /// Request to change chat
        /// After this server expects that sequence of requests:
        /// Expects ChatChangeModel. Returns nothing  
        /// </summary>
        public event Action<ChatChangeModel> OnChangeChat;
        /// <summary>
        /// Request to delete chat
        /// After this server expects that sequence of requests:
        /// Expects IdModel witch is chat id. Returns nothing  
        /// </summary>
        public event Action<IdModel> OnDeleteChat;
    }
}
