using System.Net;
using System.Net.Sockets;
using System.Text;
using NetModelsLibrary;

const int port = 8000;



Console.Write("Уведіть своє ім'я:");
string userName = Console.ReadLine();

TcpClient client = new TcpClient();
client.Connect(GetLocalIPAddress(), port);
var Network = new Network(client.GetStream());

if (userName != null)
{
    Network.WriteObject(new RequestInfoModel() { Type = RequestType.Registration });
    Network.WriteObject(new RegistrationRequestModel() { Login = userName });
}
Console.Write("Уведіть шлях до файлу:");
string FilePath = Console.ReadLine();

if (FilePath != null)
{
    Network.WriteFile(FilePath);
}
Console.Read();



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