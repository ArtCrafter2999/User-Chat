using Server;
using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using NetModelsLibrary;
using System.Xml;
#pragma warning disable SYSLIB0006

namespace Server
{
    public class CantDeserializeExeption : Exception
    {
        public CantDeserializeExeption(Type type):base($"Can't Deserialize Model. Type: '{type}'") { }
    }

    public class ClientObject
    {
        public UserModel? User { get; set; }
        public TcpClient client;
        Network? network = null;
        NetworkStream? stream = null;
        
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
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
                stream = client.GetStream();
                network = new Network(stream);
                if (stream != null)
                {
                    while (true)
                    {
                        
                        var Info = network.ReadObject<RequestInfoModel>();
                        switch (Info.Type)
                        {
                            case RequestType.Registration:
                                Registration(network.ReadObject<RegistrationRequestModel>());
                                break;
                            case RequestType.Text:
                                break;
                            case RequestType.File:
                                network.ReadFile();
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
            catch (ThreadAbortException)
            {
                if (User != null) Console.WriteLine($"Connection with {User.Name}({User.Id}) had closed");
                else Console.WriteLine($"Connection with one of Unregistered users had closed");
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
        

        public static IEnumerator<int> IdDistributor()
        {
            int id = 1;
            while (true)
            {
                yield return id;
                id++;
            }
        }
        public void Registration(RegistrationRequestModel model)
        {
            User = new UserModel() { Id = IdDistributor().Current, Name = model.Login };
            Console.WriteLine($"User {User.Name}({User.Id}) had registered");
        }
    }
}
