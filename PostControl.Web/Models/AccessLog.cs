using System.ComponentModel.DataAnnotations;

namespace PostControl.Web.Models;

public class AccessLog
{
    [Key]
    public int Id { get; set; }

    public string CardId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int PpId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Result { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}