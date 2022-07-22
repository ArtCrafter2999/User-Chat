namespace NetModelsLibrary.Models
{
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