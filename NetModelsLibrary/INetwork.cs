using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using NetModelsLibrary.Models;

namespace NetModelsLibrary
{
    public interface INetwork
    {
        public string ReadFile(string FullName, FileInfoModel Info);
        public void WriteFile(string Filepath);

        public CancellationToken? Token { get; set; }

        public T ReadObject<T>();
        public T ReadObject<T>(string raw);
        public void WriteObject<T>(T obj);
        public void WriteRequest(RequestType type)
        {
            WriteObject(new RequestInfoModel(type));
        }
        public void WriteNotify(NotifyType type)
        {
            WriteObject(new NotifyInfoModel(type));
        }
    }
}
