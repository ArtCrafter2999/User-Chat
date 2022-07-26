using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModelsLibrary
{
    public enum NotifyType
    {
        /// <summary>
        /// Notify about chat with the user has been created.
        /// Send the created ChatModel
        /// </summary>
        ChatCreated,
        /// <summary>
        /// Notify that someone has sent a message to one of the chats
        /// Send the sended MessageModel
        /// </summary>
        MessageSended,
    }
}
