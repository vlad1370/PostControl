using Microsoft.AspNetCore.SignalR;

namespace PostControl.Web.Hubs;

public class GuardHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveEvent", new
        {
            eventType = "system",
            userName = "Система",
            result = "ПОДКЛЮЧЕНИЕ",
            reason = "Новый клиент охраны подключен",
            timestamp = DateTime.Now
        });
        await base.OnConnectedAsync();
    }
}