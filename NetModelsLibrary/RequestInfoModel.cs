namespace NetModelsLibrary
{
    public enum RequestType
    {
        Registration,
        Text,
        File,
    }
    public class RequestInfoModel
    {
        public RequestType Type { get; set; }
    }
}