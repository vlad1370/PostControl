namespace PostControl.Shared.Models;

public class AccessResponse
{
    public bool Allowed { get; set; }
    public string Message { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}