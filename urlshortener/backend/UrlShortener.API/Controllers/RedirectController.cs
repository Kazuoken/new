using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Services;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("r")]
public class RedirectController : ControllerBase
{
    private readonly IUrlService _urlService;
    private readonly ILogger<RedirectController> _logger;

    public RedirectController(IUrlService urlService, ILogger<RedirectController> logger)
    {
        _urlService = urlService;
        _logger = logger;
    }

    // GET /r/{code}
    [HttpGet("{code}")]
    public async Task<IActionResult> Redirect(string code)
    {
        var entity = await _urlService.GetByCodeAsync(code);

        if (entity == null)
            return NotFound(new { message = $"Short URL '{code}' not found." });

        if (!entity.IsActive)
            return Gone();

        if (entity.ExpiresAt.HasValue && entity.ExpiresAt.Value < DateTime.UtcNow)
            return Gone();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        _ = _urlService.IncrementClickAsync(code, ip); // fire-and-forget

        _logger.LogInformation("Redirecting code={Code} -> {Url}", code, entity.OriginalUrl);
        return Redirect(entity.OriginalUrl);
    }

    private ObjectResult Gone() =>
        StatusCode(410, new { message = "This short URL is no longer active or has expired." });
}
