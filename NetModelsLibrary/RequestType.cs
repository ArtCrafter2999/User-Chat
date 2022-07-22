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
        /// After this server expects that sequence of requests:
        /// Expects UserCreationModel returns ResoultModel
        /// </summary>
        Registration,
        /// <summary>
        /// After this server expects MessageModel
        /// </summary>
        Message,
        /// <summary>
        /// After this server expects that sequence of requests:
        /// Expects AuthModel returns ResoultModel
        /// </summary>
        Auth,
        /// <summary>
        /// After this server not expects anything, instred it returns AllChatsModel for current user
        /// </summary>
        GetAllChats,
        /// <summary>
        /// After this server expects that sequence of requests:
        /// Expects AuthModel returns ResoultModel 
        /// and then, if success, returns ChatModel  
        /// </summary>
        CreateChat,
        /// <summary>
        /// After this server expects that sequence of requests:
        /// Expects SearchModel returns AllUsersModel 
        /// </summary>
        SearchUsers,
    }
}
