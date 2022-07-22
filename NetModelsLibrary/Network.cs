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
    public class Network
    {
        NetworkStream _stream;
        public Network(NetworkStream stream)
        {
            _stream = stream;
        }

        public readonly Regex FileRegEx = new Regex(@"([^\\]+)\.([^\.]+$)");

        public string ReadFile(string FullName, FileInfoModel Info)
        {
            using (var Fstream = new FileStream(FullName, FileMode.CreateNew, FileAccess.Write))
            {
                bool IsLast = false;
                do
                {
                    var model = ReadObject<FileModel>();
                    Fstream.Write(model.Data, 0, model.DataSize);
                    IsLast = model.PackageIndex == Info.PackageCount - 1;
                } while (!IsLast);
            }
            return FullName;
        }

        public void WriteFile(string Filepath)
        {
            using (var Fstream = new FileStream(Filepath, FileMode.Open, FileAccess.Read))
            {
                int PackcagesCount = (int)(Fstream.Length / 1024);
                if (Fstream.Length % 1024 > 0) PackcagesCount++;
                Match match = FileRegEx.Match(Filepath);
                WriteObject(new FileInfoModel() 
                {
                    DataSize = Fstream.Length,
                    Name = match.Groups[1].Value,
                    Format = match.Groups[2].Value,
                    PackageCount = PackcagesCount
                });
                for (int i = 0; i < PackcagesCount; i++)
                {
                    byte[] Data = new byte[1024];
                    int lenght = Fstream.Read(Data, 0, 1024);
                    WriteObject(new FileModel()
                    {
                        Data = Data,
                        DataSize = lenght,
                        PackageIndex = i
                    });
                }
            }
        }

        public T ReadObject<T>()
        {
            if (_stream != null)
            {
                var message = new BinaryReader(_stream).ReadString();
                var Serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                T? Model;
                using (TextReader reader = new StringReader(message))
                    Model = (T?)Serializer.Deserialize(reader);
                if (Model != null) return Model;
            }
            throw new Exception();
        }
        public void WriteObject<T>(T obj)
        {
            if (_stream != null && obj != null)
            {
                var TempStream = new MemoryStream(); //тимчасовий потік для збереження сереалізованих даниїх
                var Serializer = new System.Xml.Serialization.XmlSerializer(typeof(T)); //серіалайзер
                Serializer.Serialize(TempStream, obj);//серіалізовую в тимчасовий потік
                TempStream.Position = 0; // повертаюсь до початку
                new BinaryWriter(_stream).Write( //записую в мережевий потік той рядок
                    new StreamReader(TempStream, Encoding.UTF8).ReadToEnd() // який отримуєтсья з тимчасового потіка
                );
            }
        }
        public void WriteRequest(RequestType type)
        {
            WriteObject(new RequestInfoModel(type));
        }
    }
}
