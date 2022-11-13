using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using UrlShortener.Application;
using UrlShortener.Application.Services;
using UrlShortener.Domain;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ShortenedEntryController : ControllerBase
{
    private readonly IShortenedEntryService _shortenedEntryService;

    public ShortenedEntryController(IShortenedEntryService shortenedEntryService)
    {
        _shortenedEntryService = shortenedEntryService;
    }

    [HttpGet("{alias}")]
    [ProducesResponseType(StatusCodes.Status302Found, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShortenedEntry([FromRoute] string alias, CancellationToken cancellationToken)
    {
        var optionalEntry = await _shortenedEntryService.GetByAliasAsync(alias, cancellationToken);

        return optionalEntry
            .Some<IActionResult>(entry => Redirect(entry.Url))
            .None(NotFound);
    }

    [HttpPut("api/v1/shortenedEntry")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateShortenedEntryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateShortenedEntry(
        [FromBody] CreateShortenedEntryRequest request, CancellationToken cancellationToken)
    {
        var optionalEntry = await _shortenedEntryService.CreateAsync(request, cancellationToken);

        return optionalEntry
            .Some<IActionResult>(entry =>
            {
                var response = MapCreateShortenedEntryResponse(entry);
                return CreatedAtAction(nameof(GetShortenedEntry), new { alias = response.Alias }, response);
            })
            .None(BadRequest("Alias already exists"));
    }

    private static CreateShortenedEntryResponse MapCreateShortenedEntryResponse(ShortenedEntry entry)
    {
        return new CreateShortenedEntryResponse(entry.Alias, entry.Url, entry.Creation, entry.Expiration);
    }
}