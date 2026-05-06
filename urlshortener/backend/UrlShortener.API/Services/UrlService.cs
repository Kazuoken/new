using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.API.Data;
using UrlShortener.API.Models;
using UrlShortener.API.Models.DTOs;

namespace UrlShortener.API.Services;

public class UrlService : IUrlService
{
    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<UrlService> _logger;

    public UrlService(AppDbContext db, IMemoryCache cache, ILogger<UrlService> logger)
    {
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    public async Task<UrlResponse> CreateAsync(CreateUrlRequest request, string baseUrl)
    {
        string code;

        if (!string.IsNullOrWhiteSpace(request.CustomCode))
        {
            code = request.CustomCode.Trim();
            var exists = await _db.ShortenedUrls.AnyAsync(u => u.Code == code);
            if (exists)
                throw new InvalidOperationException($"Code '{code}' is already taken.");
        }
        else
        {
            code = await GenerateUniqueCodeAsync();
        }

        var entity = new ShortenedUrl
        {
            Code = code,
            OriginalUrl = request.OriginalUrl,
            Title = request.Title,
            ExpiresAt = request.ExpiresAt,
            CreatedAt = DateTime.UtcNow
        };

        _db.ShortenedUrls.Add(entity);
        await _db.SaveChangesAsync();

        return MapToResponse(entity, baseUrl);
    }

    public async Task<ShortenedUrl?> GetByCodeAsync(string code)
    {
        var cacheKey = $"url:{code}";

        if (_cache.TryGetValue(cacheKey, out ShortenedUrl? cached))
            return cached;

        var entity = await _db.ShortenedUrls
            .FirstOrDefaultAsync(u => u.Code == code && u.IsActive);

        if (entity != null)
        {
            _cache.Set(cacheKey, entity, TimeSpan.FromMinutes(10));
        }

        return entity;
    }

    public async Task<UrlResponse?> GetByIdAsync(long id, string baseUrl)
    {
        var entity = await _db.ShortenedUrls.FindAsync(id);
        return entity == null ? null : MapToResponse(entity, baseUrl);
    }

    public async Task<PagedResult<UrlResponse>> GetAllAsync(int page, int pageSize, string baseUrl)
    {
        var query = _db.ShortenedUrls.OrderByDescending(u => u.CreatedAt);
        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<UrlResponse>
        {
            Items = items.Select(u => MapToResponse(u, baseUrl)),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<UrlResponse?> UpdateAsync(long id, UpdateUrlRequest request, string baseUrl)
    {
        var entity = await _db.ShortenedUrls.FindAsync(id);
        if (entity == null) return null;

        if (request.Title != null) entity.Title = request.Title;
        if (request.ExpiresAt.HasValue) entity.ExpiresAt = request.ExpiresAt;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;

        await _db.SaveChangesAsync();
        _cache.Remove($"url:{entity.Code}");

        return MapToResponse(entity, baseUrl);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await _db.ShortenedUrls.FindAsync(id);
        if (entity == null) return false;

        _cache.Remove($"url:{entity.Code}");
        _db.ShortenedUrls.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task IncrementClickAsync(string code, string? ip)
    {
        var entity = await _db.ShortenedUrls.FirstOrDefaultAsync(u => u.Code == code);
        if (entity == null) return;

        entity.ClickCount++;
        entity.LastAccessedAt = DateTime.UtcNow;
        entity.LastAccessedIp = ip;
        await _db.SaveChangesAsync();
        _cache.Remove($"url:{code}");
    }

    // --- Helpers ---

    private async Task<string> GenerateUniqueCodeAsync()
    {
        string code;
        do
        {
            // Get the next ID to encode (Base62)
            var maxId = await _db.ShortenedUrls.AnyAsync()
                ? await _db.ShortenedUrls.MaxAsync(u => u.Id)
                : 0;
            code = ToBase62(maxId + 1 + Random.Shared.Next(1000));
        }
        while (await _db.ShortenedUrls.AnyAsync(u => u.Code == code));

        return code;
    }

    private static string ToBase62(long number)
    {
        if (number == 0) return "0";
        var result = new Stack<char>();
        while (number > 0)
        {
            result.Push(Base62Chars[(int)(number % 62)]);
            number /= 62;
        }
        // Pad to minimum 6 chars with random suffix for uniqueness
        var code = new string(result.ToArray());
        return code.Length >= 6 ? code[..7] : code.PadRight(6, Base62Chars[Random.Shared.Next(62)]);
    }

    private static UrlResponse MapToResponse(ShortenedUrl entity, string baseUrl) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        OriginalUrl = entity.OriginalUrl,
        ShortUrl = $"{baseUrl.TrimEnd('/')}/r/{entity.Code}",
        Title = entity.Title,
        CreatedAt = entity.CreatedAt,
        ExpiresAt = entity.ExpiresAt,
        ClickCount = entity.ClickCount,
        LastAccessedAt = entity.LastAccessedAt,
        IsActive = entity.IsActive
    };
}
