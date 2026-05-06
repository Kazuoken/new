using UrlShortener.API.Models;
using UrlShortener.API.Models.DTOs;

namespace UrlShortener.API.Services;

public interface IUrlService
{
    Task<UrlResponse> CreateAsync(CreateUrlRequest request, string baseUrl);
    Task<ShortenedUrl?> GetByCodeAsync(string code);
    Task<UrlResponse?> GetByIdAsync(long id, string baseUrl);
    Task<PagedResult<UrlResponse>> GetAllAsync(int page, int pageSize, string baseUrl);
    Task<UrlResponse?> UpdateAsync(long id, UpdateUrlRequest request, string baseUrl);
    Task<bool> DeleteAsync(long id);
    Task IncrementClickAsync(string code, string? ip);
}
