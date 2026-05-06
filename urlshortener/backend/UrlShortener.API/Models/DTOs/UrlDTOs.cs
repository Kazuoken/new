using System.ComponentModel.DataAnnotations;

namespace UrlShortener.API.Models.DTOs;

public class CreateUrlRequest
{
    [Required]
    [Url(ErrorMessage = "Please provide a valid URL.")]
    public string OriginalUrl { get; set; } = string.Empty;

    public string? CustomCode { get; set; }

    public string? Title { get; set; }

    public DateTime? ExpiresAt { get; set; }
}

public class UrlResponse
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortUrl { get; set; } = string.Empty;
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public long ClickCount { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateUrlRequest
{
    public string? Title { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool? IsActive { get; set; }
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
