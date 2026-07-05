using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PostControl.Web.Data;
using System.Text;

namespace PostControl.Web.Pages.Reports;

public class ExportModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public ExportModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> OnGetAsync(DateTime startDate, DateTime endDate)
    {
        var logs = await _db.AccessLogs
            .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate.AddDays(1))
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("Дата;Сотрудник;ID карты;Результат;Причина");

        foreach (var log in logs)
        {
            csv.AppendLine($"{log.Timestamp};{log.UserName};{log.CardId};{log.Result};{log.Reason}");
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        return File(bytes, "text/csv; charset=utf-8", $"report_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv");
    }
}