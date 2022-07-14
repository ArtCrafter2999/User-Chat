namespace NetModelsLibrary.Models
{
    public enum RequestType
    {
        Registration,
        Message,
        Auth,
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