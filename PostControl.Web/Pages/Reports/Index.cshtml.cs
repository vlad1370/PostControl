using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PostControl.Web.Data;
using PostControl.Web.Models;

namespace PostControl.Web.Pages.Reports;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<AccessLog> Logs { get; set; } = new();
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7);
    public DateTime EndDate { get; set; } = DateTime.Today;
    public string Search { get; set; } = string.Empty;

    public async Task OnGetAsync(DateTime? startDate, DateTime? endDate, string search)
    {
        StartDate = startDate ?? DateTime.Today.AddDays(-7);
        EndDate = endDate ?? DateTime.Today;
        Search = search ?? string.Empty;

        var query = _db.AccessLogs.AsQueryable();

        query = query.Where(l => l.Timestamp >= StartDate && l.Timestamp <= EndDate.AddDays(1));

        if (!string.IsNullOrEmpty(Search))
        {
            query = query.Where(l =>
                l.UserName.Contains(Search) ||
                l.CardId.Contains(Search));
        }

        Logs = await query
            .OrderByDescending(l => l.Timestamp)
            .Take(500)
            .ToListAsync();
    }
}