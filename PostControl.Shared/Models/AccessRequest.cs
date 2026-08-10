namespace PostControl.Shared.Models;

public class AccessRequest
{
    public string CardId { get; set; } = string.Empty;
    public int PpId { get; set; }
    public DateTime Timestamp { get; set; }
}