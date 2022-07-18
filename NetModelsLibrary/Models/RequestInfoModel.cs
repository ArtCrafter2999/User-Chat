namespace NetModelsLibrary.Models
{
    public enum RequestType
    {
        /// <summary>
        /// After this server expects that sequence of requests:
        /// Expects AuthModel returns ResoultModel
        /// If success expects UserModel returns ResoultModel
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
        GetAllChats,
    }
    public class RequestInfoModel
    {
        public RequestType Type { get; set; }
        public RequestInfoModel(RequestType type)
        {
            Type = type;
        }
        public RequestInfoModel(){}
    }
}