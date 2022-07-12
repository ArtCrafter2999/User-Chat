using Server;
using System.Net;
using System.Net.Sockets;

const int port = 8000;
TcpListener? listener = null;
try
{
    listener = new TcpListener(GetLocalIPAddress(), port);
    listener.Start();
    Console.WriteLine("Waiting for connections...");

    while (true)
    {
        TcpClient client = listener.AcceptTcpClient();
        ClientObject clientObject = new ClientObject(client);

        // создаем новый поток для обслуживания нового клиента
        clientObject.Start();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    if (listener != null)
        listener.Stop();
}


IPAddress GetLocalIPAddress()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            return ip;
        }
    }
    throw new Exception("No network adapters with an IPv4 address in the system!");
}