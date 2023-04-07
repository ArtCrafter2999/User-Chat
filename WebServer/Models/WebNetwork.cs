using Microsoft.AspNetCore.SignalR;
using NetModelsLibrary;
using NetModelsLibrary.Models;

namespace WebServer.Models
{
    public class WebNetwork : INetwork
    {
        public CancellationToken? Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private IHubContext _hubContext { get; set; }

        private string _hubMethod { get; set; } = "";
        private string _userId { get; set; }

        public WebNetwork(IHubContext hubContext, string userId)
        {
            _hubContext = hubContext;
            _userId = userId;
        }

        public string ReadFile(string FullName, FileInfoModel Info)
        {
            throw new NotImplementedException();
        }

        public T ReadObject<T>()
        {
            throw new NotImplementedException();
        }

        public T ReadObject<T>(string raw)
        {
            throw new NotImplementedException();
        }

        public void WriteFile(string Filepath)
        {
            throw new NotImplementedException();
        }

        public void WriteObject<T>(T obj)
        {
            _hubContext.Clients.User(_userId).SendAsync(_hubMethod, obj);
        }

        public void WriteRequest(RequestType type)
        {
            _hubMethod = type.ToString();
        }

        public void WriteNotify(NotifyType type)
        {
            _hubMethod = type.ToString();
        }
    }
}
