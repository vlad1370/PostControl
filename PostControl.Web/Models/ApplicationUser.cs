using Microsoft.AspNetCore.Identity;

namespace PostControl.Web.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string CardId { get; set; } = string.Empty;
    public string AccessLevel { get; set; } = "employee";
    public string TimeWindows { get; set; } = "09:00-18:00";
    public bool IsBlocked { get; set; }
    public DateTime? BlockedUntil { get; set; }
    public int FailedAttempts { get; set; }
    public DateTime? LastFailedAttempt { get; set; }
}