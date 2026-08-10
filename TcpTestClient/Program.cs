using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

var client = new TcpClient("localhost", 8888);
var sslStream = new SslStream(client.GetStream(), false, (sender, cert, chain, errors) => true);
await sslStream.AuthenticateAsClientAsync("localhost");

var writer = new StreamWriter(sslStream);
var reader = new StreamReader(sslStream);

writer.WriteLine("{\"CardId\":\"TEST001\",\"PpId\":1}");
writer.Flush();

var response = reader.ReadLine();
Console.WriteLine("Ответ от сервера: " + response);

Console.ReadKey();