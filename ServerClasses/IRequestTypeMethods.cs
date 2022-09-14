using NetModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerClasses
{
    public interface IRequestTypeMethods
    {
        /// <summary>
        /// Registration request which contain login username and password in MD5
        /// After this server expects that sequence of requests:
        /// Expects UserCreationModel returns ResoultModel
        /// </summary>
        public void OnRegistration(UserCreationModel model);
        /// <summary>
        /// Request for send message from the user selected chat
        /// After this server expects MessageModel
        /// </summary>
        public void OnSendMessage(MessageModel model);
        /// <summary>
        /// Authorization request which contain login and password in MD5
        /// After this server expects that sequence of requests:
        /// Expects AuthModel returns ResoultModel
        /// </summary>
        public void OnAuth(AuthModel model);
        /// <summary>
        /// Request to get all chats which the user is a member
        /// After this server not expects anything, instred it returns AllChatsModel for current user
        /// </summary>
        public void OnGetAllChats();
        /// <summary>
        /// Request to create chat
        /// After this server expects that sequence of requests:
        /// Expects ChatCreateModel returns ResoultModel 
        /// </summary>
        public void OnCreateChat(ChatCreationModel model);
        /// <summary>
        /// Searching for a new user to add to a new chat
        /// After this server expects that sequence of requests:
        /// Expects SearchModel returns AllUsersModel 
        /// </summary>
        public void OnSearchUsers(SearchModel model);
        /// <summary>
        /// Page by page to get messages
        /// After this server expects that sequence of requests:
        /// Expects GetMessagesInfoModel returns MessagesPageModel
        /// </summary>
        public void OnGetPageOfMessages(GetMessagesInfoModel model);
        /// <summary>
        /// Get all unreaded messages and mark them readed
        /// After this server expects that sequence of requests:
        /// Expects IdModel witch is chat id and returns MessagesPageModel
        /// </summary>
        public void OnReadUnreaded(IdModel model);
        /// <summary>
        /// Mark all unreaded messages as readed
        /// After this server expects that sequence of requests:
        /// Expects IdModel witch is chat id. Returns nothing
        /// </summary>
        public void OnMarkReaded(IdModel model);
        /// <summary>
        /// Request to change chat
        /// After this server expects that sequence of requests:
        /// Expects ChatChangeModel. Returns nothing  
        /// </summary>
        public void OnChangeChat(ChatChangeModel model);
        /// <summary>
        /// Request to delete chat
        /// After this server expects that sequence of requests:
        /// Expects IdModel witch is chat id. Returns nothing  
        /// </summary>
        public void OnDeleteChat(IdModel model);
    }
}
