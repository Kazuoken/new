using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Models.DTOs;
using UrlShortener.API.Services;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlController : ControllerBase
{
    private readonly IUrlService _urlService;
    private readonly ILogger<UrlController> _logger;

    public UrlController(IUrlService urlService, ILogger<UrlController> logger)
    {
        _urlService = urlService;
        _logger = logger;
    }

    // POST /api/url
    [HttpPost]
    public async Task<IActionResult> CreateUrl([FromBody] CreateUrlRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await _urlService.CreateAsync(request, baseUrl);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating URL");
            return StatusCode(500, new { message = "An error occurred while creating the URL." });
        }
    }

    // GET /api/url/manage — all URLs (paginated)
    [HttpGet("manage")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var result = await _urlService.GetAllAsync(page, pageSize, baseUrl);
        return Ok(result);
    }

    // GET /api/url/{id}
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var result = await _urlService.GetByIdAsync(id, baseUrl);
        return result == null ? NotFound() : Ok(result);
    }

    // PATCH /api/url/{id}
    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUrlRequest request)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var result = await _urlService.UpdateAsync(id, request, baseUrl);
        return result == null ? NotFound() : Ok(result);
    }

    // DELETE /api/url/{id}
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _urlService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
