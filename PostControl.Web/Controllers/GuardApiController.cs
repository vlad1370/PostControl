using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PostControl.Web.Data;
using PostControl.Web.Hubs;
using PostControl.Web.Models;

namespace PostControl.Web.Controllers;

[ApiController]
[Route("api/guard")]
public class GuardApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IHubContext<GuardHub> _hub;

    public GuardApiController(ApplicationDbContext db, IHubContext<GuardHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    [HttpPost("access")]
    public async Task<IActionResult> CheckAccess([FromBody] AccessRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.CardId == request.CardId);

        if (user == null)
            return Ok(new { allowed = false, message = "Карта не найдена" });

        if (user.IsBlocked)
        {
            await Notify(new
            {
                eventType = "access_denied",
                userName = user.FullName,
                result = "ЗАПРЕЩЁН",
                reason = "Пользователь заблокирован",
                timestamp = DateTime.Now
            });
            return Ok(new { allowed = false, message = "Пользователь заблокирован", userName = user.FullName });
        }

        // Проверка времени доступа
        var now = DateTime.Now;
        var timeWindows = user.TimeWindows.Split('-');
        var start = TimeSpan.Parse(timeWindows[0]);
        var end = TimeSpan.Parse(timeWindows[1]);

        if (now.TimeOfDay < start || now.TimeOfDay > end)
        {
            user.FailedAttempts++;
            if (user.FailedAttempts >= 3)
            {
                user.IsBlocked = true;
                await _db.SaveChangesAsync();

                await Notify(new
                {
                    eventType = "user_blocked",
                    userName = user.FullName,
                    result = "БЛОКИРОВКА",
                    reason = "3 неудачные попытки",
                    timestamp = DateTime.Now
                });
                return Ok(new { allowed = false, message = "Пользователь заблокирован за 3 нарушения", userName = user.FullName });
            }
            await _db.SaveChangesAsync();

            await Notify(new
            {
                eventType = "access_denied",
                userName = user.FullName,
                result = "ЗАПРЕЩЁН",
                reason = $"Время доступа истекло (работаем с {start} до {end})",
                timestamp = DateTime.Now
            });
            return Ok(new { allowed = false, message = "Время доступа истекло", userName = user.FullName });
        }

        // Успешный проход
        user.FailedAttempts = 0;
        await _db.SaveChangesAsync();

        await Notify(new
        {
            eventType = "access_granted",
            userName = user.FullName,
            result = "РАЗРЕШЁН",
            reason = "Доступ разрешён",
            timestamp = DateTime.Now
        });

        return Ok(new { allowed = true, message = "Проход разрешён", userName = user.FullName });
    }

    [HttpPost("unblock")]
    public async Task<IActionResult> UnblockUser([FromBody] UnblockRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.CardId == request.CardId);

        if (user == null)
            return Ok(new { message = "Пользователь не найден" });

        if (!user.IsBlocked)
            return Ok(new { message = $"Пользователь {user.FullName} уже разблокирован" });

        user.IsBlocked = false;
        user.FailedAttempts = 0;
        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("ReceiveEvent", new
        {
            eventType = "system",
            userName = user.FullName,
            result = "РАЗБЛОКИРОВАН",
            reason = "Пользователь разблокирован",
            timestamp = DateTime.Now
        });

        return Ok(new { message = $"Пользователь {user.FullName} разблокирован" });
    }

    private async Task Notify(object eventData)
    {
        await _hub.Clients.All.SendAsync("ReceiveEvent", eventData);
    }
}

public class AccessRequest
{
    public string CardId { get; set; }
    public int PpId { get; set; }
}

public class UnblockRequest
{
    public string CardId { get; set; }
}