using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

Console.OutputEncoding = Encoding.UTF8;

var client = new TcpClient("localhost", 8888);
var sslStream = new SslStream(client.GetStream(), false, (sender, cert, chain, errors) => true);
await sslStream.AuthenticateAsClientAsync("localhost");

var writer = new StreamWriter(sslStream);
var reader = new StreamReader(sslStream);

writer.WriteLine("{\"CardId\":\"TEST001\",\"PpId\":1}");
writer.Flush();

var response = reader.ReadLine();

if (!string.IsNullOrEmpty(response))
{
    using var doc = JsonDocument.Parse(response);
    var root = doc.RootElement;

    var allowed = root.GetProperty("Allowed").GetBoolean();
    var message = root.GetProperty("Message").GetString();
    var userName = root.GetProperty("UserName").GetString();

    Console.WriteLine("=== Ответ от сервера ===");
    Console.WriteLine($"Разрешён: {allowed}");
    Console.WriteLine($"Сообщение: {message}");
    Console.WriteLine($"Пользователь: {userName}");
}
else
{
    Console.WriteLine("Ответ пуст");
}

Console.ReadKey();


/*
cd C:\Users\Admin\source\repos\PostControl\PostControl.Server
dotnet run
cd C:\Users\Admin\source\repos\PostControl\TcpTestClient
dotnet run
*/