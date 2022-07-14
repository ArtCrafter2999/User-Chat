using Server;
using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using NetModelsLibrary;
using System.Xml;
using NetModelsLibrary.Models;
#pragma warning disable SYSLIB0006

namespace Server
{
    public class CantDeserializeExeption : Exception
    {
        public CantDeserializeExeption(Type type):base($"Can't Deserialize Model. Type: '{type}'") { }
    }

    public class ClientObject
    {
        public int? UserId { get; set; }
        public TcpClient client;
        public Network network;
        NetworkStream stream;
        public DataBaseHandler handler;
        
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
            stream = client.GetStream();
            network = new Network(stream);
            handler = new DataBaseHandler(this); 
        }
        private Thread? Thread = null;
        public void Stop()
        {
            Thread?.Abort();
        }
        public void Start()
        {
            Thread = new Thread(new ThreadStart(Process));
            Thread.Start();
        }
        public void Process()
        {
            try
            {
                Console.WriteLine("Unknown User had connected successfully");
                if (stream != null)
                {
                    while (true)
                    {
                        var Info = network.ReadObject<RequestInfoModel>();
                        switch (Info.Type)
                        {
                            case RequestType.Registration:
                                handler.Registration(network.ReadObject<AuthModel>());
                                break;
                            case RequestType.Message:
                                handler.Message(network.ReadObject<MessageModel>());
                                break;
                            case RequestType.Auth:
                                handler.Auth(network.ReadObject<AuthModel>());
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
            catch (OperationFailureExeption ex)
            {
                network.WriteObject(new ResoultModel() { RequestType = ex.RequestType, Success = false, Message = ex.Message});
                Console.WriteLine(ex.Message);
            }
            catch (IOException)
            {
                Console.WriteLine($"Користувач '{handler.LoginFromUserId(UserId ?? -1)}'({UserId ?? 0}) розірвав з'єднання");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}
