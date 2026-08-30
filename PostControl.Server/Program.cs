using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using PostControl.Shared.Models;

var listener = new TcpListener(IPAddress.Any, 8888);
listener.Start();
Console.WriteLine("TCP-сервер с TLS запущен на порту 8888");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    _ = HandleClientAsync(client);
}

static async Task HandleClientAsync(TcpClient client)
{
    try
    {
        using (client)
        {
            // ====== ПРОВЕРКА СЕРТИФИКАТА ======
            string certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "server.pfx");
            if (!File.Exists(certPath))
            {
                Console.WriteLine($"[TCP] Сертификат не найден: {certPath}");
                return;
            }
            Console.WriteLine($"[TCP] Сертификат загружен: {certPath}");

            // Загружаем сертификат
            var certificate = new X509Certificate2(certPath, "123456");
            var sslStream = new SslStream(client.GetStream(), false);
            await sslStream.AuthenticateAsServerAsync(certificate);

            // Читаем данные
            var buffer = new byte[4096];
            int bytesRead = await sslStream.ReadAsync(buffer);
            var requestJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var request = JsonSerializer.Deserialize<AccessRequest>(requestJson);

            Console.WriteLine($"[TCP] Получен запрос: CardId={request?.CardId}");

            // Формируем ответ
            var response = new AccessResponse
            {
                Allowed = true,
                Message = "Проход разрешён (TLS)",
                UserName = "Тестовый пользователь"
            };

            var responseJson = JsonSerializer.Serialize(response);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);
            await sslStream.WriteAsync(responseBytes);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[TCP] Ошибка: {ex.Message}");
    }
}