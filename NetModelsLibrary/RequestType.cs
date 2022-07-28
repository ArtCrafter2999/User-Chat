using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModelsLibrary
{
    public enum RequestType
    {
        /// <summary>
        /// Registration request which contain login username and password in MD5
        /// After this server expects that sequence of requests:
        /// Expects UserCreationModel returns ResoultModel
        /// </summary>
        Registration,
        /// <summary>
        /// Request for send message from the user selected chat
        /// After this server expects MessageModel and returns the same message but with id
        /// </summary>
        SendMessage,
        /// <summary>
        /// Authorization request which contain login and password in MD5
        /// After this server expects that sequence of requests:
        /// Expects AuthModel returns ResoultModel
        /// </summary>
        Auth,
        /// <summary>
        /// Request to get all chats which the user is a member
        /// After this server not expects anything, instred it returns AllChatsModel for current user
        /// </summary>
        GetAllChats,
        /// <summary>
        /// Request to create chat
        /// After this server expects that sequence of requests:
        /// Expects ChatCreateModel returns ResoultModel 
        /// </summary>
        CreateChat,
        /// <summary>
        /// Searching for a new user to add to a new chat
        /// After this server expects that sequence of requests:
        /// Expects SearchModel returns AllUsersModel 
        /// </summary>
        SearchUsers,
        /// <summary>
        /// Page by page to get messages
        /// After this server expects that sequence of requests:
        /// Expects GetMessagesInfoModel returns MessagesPageModel
        /// </summary>
        GetPageOfMessages,
        /// <summary>
        /// Get all unreaded messages and mark them readed
        /// After this server expects that sequence of requests:
        /// Expects IdModel witch is chat id and returns MessagesPageModel
        /// </summary>
        ReadUnreaded,
        /// <summary>
        /// Mark all unreaded messages as readed
        /// After this server expects that sequence of requests:
        /// Expects IdModel witch is chat id. Returns nothing
        /// </summary>
        MarkReaded,
        /// <summary>
        /// Request to change chat
        /// After this server expects that sequence of requests:
        /// Expects ChatChangeModel. Returns nothing  
        /// </summary>
        ChangeChat,
        /// <summary>
        /// Request to delete chat
        /// After this server expects that sequence of requests:
        /// Expects IdModel witch is chat id. Returns nothing  
        /// </summary>
        DeleteChat
    }
}
