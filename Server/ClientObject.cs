﻿using Server;
using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using NetModelsLibrary;
using System.Xml;
using NetModelsLibrary.Models;
using ServerDatabase;
#pragma warning disable SYSLIB0006

namespace Server
{
    public class CantDeserializeExeption : Exception
    {
        public CantDeserializeExeption(Type type) : base($"Can't Deserialize Model. Type: '{type}'") { }
    }

    public class ClientObject
    {
        public int? UserId => handler?.User.Id;
        public User User => handler?.User;
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
                        try
                        {
                            var Info = network.ReadObject<RequestInfoModel>();
                            switch (Info.Type)
                            {
                                case RequestType.Registration:
                                    handler.Registration(network.ReadObject<UserCreationModel>());
                                    break;
                                case RequestType.SendMessage:
                                    handler.SendMessage(network.ReadObject<MessageModel>());
                                    break;
                                case RequestType.Auth:
                                    handler.Auth(network.ReadObject<AuthModel>());
                                    break;
                                case RequestType.GetAllChats:
                                    handler.GetAllChats();
                                    break;
                                case RequestType.CreateChat:
                                    handler.CreateChat(network.ReadObject<ChatCreationModel>());
                                    break;
                                case RequestType.SearchUsers:
                                    handler.SearchUsers(network.ReadObject<SearchModel>());
                                    break;
                                case RequestType.GetPageOfMessages:
                                    handler.GetPageOfMessages(network.ReadObject<GetMessagesInfoModel>());
                                    break;
                                case RequestType.ReadUnreaded:
                                    handler.ReadUnreaded(network.ReadObject<IdModel>());
                                    break;
                                case RequestType.MarkReaded:
                                    handler.MarkReaded(network.ReadObject<IdModel>());
                                    break;
                                case RequestType.ChangeChat:
                                    handler.ChangeChat(network.ReadObject<ChatChangeModel>());
                                    break;
                                case RequestType.DeleteChat:
                                    handler.DeleteChat(network.ReadObject<IdModel>());
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (OperationFailureExeption ex)
                        {
                            network.WriteObject(new ResultModel(ex.RequestType, false, ex.Message));
                            Console.WriteLine(ex.Message);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.InnerException);
                            Console.WriteLine(ex.StackTrace);
                        }
                        catch (IOException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.InnerException);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"User '{User.Login}'({User.Id}) disconnected");
                handler.UserOffline();
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
