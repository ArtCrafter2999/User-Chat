using System.Net;
using System.Net.Sockets;
using System.Text;
using NetModelsLibrary;
using NetModelsLibrary.Models;

const int port = 8000;

TcpClient client = new TcpClient();
client.Connect(GetLocalIPAddress(), port);
var network = new Network(client.GetStream());

ResoultModel response;
string Login;
do
{
    Login = Read("Login: ", true);
    string PasswordMD5 = MD5.CreateMD5(Read("Password: ", true));

    network.WriteObject(new RequestInfoModel(RequestType.Registration));
    network.WriteObject(new AuthModel() { Login = Login, PasswordMD5 = PasswordMD5 });
    response = network.ReadObject<ResoultModel>();
    Console.WriteLine(response.Message);
} while (response.Success == false);
do
{
    string UserName = Read("UserName: ");

    network.WriteObject(new UserDataModel() { Login = Login, Name = UserName });
    response = network.ReadObject<ResoultModel>();
    Console.WriteLine(response.Message);
} while (response.Success == false);

Console.ReadKey();

string Read(string message, bool spasesLimit = false)
{
    string input;
    do
    {
        Console.Write(message);
        input = Console.ReadLine();
    } while (input == null && (!spasesLimit || !input.Contains(' ')));
    return input;
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