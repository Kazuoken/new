namespace UrlShortener.API.Models;

public class ShortenedUrl
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public long ClickCount { get; set; } = 0;
    public string? LastAccessedIp { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
