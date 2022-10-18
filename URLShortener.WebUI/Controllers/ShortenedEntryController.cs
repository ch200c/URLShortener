using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using URLShortener.Application;
using URLShortener.Application.Services;
using URLShortener.Application.Persistence;
using URLShortener.Domain;

namespace URLShortener.WebUI.Controllers
{
    // TODO versioning, validation
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class ShortenedEntryController : ControllerBase
    {
        private readonly ILogger<ShortenedEntryController> _logger;
        private readonly IShortenedEntryRepository _shortenedEntryRepository;
        private readonly IShortenedEntryCreationService _shortenedEntryCreationService;

        public ShortenedEntryController(
            ILogger<ShortenedEntryController> logger,
            IShortenedEntryRepository shortenedEntryRepository,
            IShortenedEntryCreationService shortenedEntryCreationService)
        {
            _logger = logger;
            _shortenedEntryRepository = shortenedEntryRepository;
            _shortenedEntryCreationService = shortenedEntryCreationService;
        }

        [HttpGet("{alias}")]
        [ProducesResponseType(StatusCodes.Status302Found, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShortenedEntry([FromRoute] string alias, CancellationToken cancellationToken)
        {
            // TODO: Application service + cache
            var optionalEntry = await _shortenedEntryRepository.GetByAliasAsync(alias, cancellationToken);

            // TODO compare with nulls
            //optionalEntry
            //    .Some<IActionResult>(entry => Redirect(entry.Url))
            //    .None(NotFound);

            return optionalEntry.Match<IActionResult>(
                entry => Redirect(entry.Url),
                () => NotFound());  // TODO method group 
        }

        [HttpPut("api/v1/shortenedEntry")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateShortenedEntryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateShortenedEntry(
            [FromBody] CreateShortenedEntryRequest request, CancellationToken cancellationToken)
        {
            var optionalEntry = await _shortenedEntryCreationService.CreateAsync(request, cancellationToken);

            return optionalEntry.Match<IActionResult>(
                entry =>
                {
                    var response = MapCreateShortenedEntryResponse(entry);
                    return CreatedAtAction(nameof(GetShortenedEntry), new { alias = response.Alias }, response);
                },
                () =>
                {
                    return BadRequest("Alias already exists");
                });
        }

        private static CreateShortenedEntryResponse MapCreateShortenedEntryResponse(ShortenedEntry entry)
        {
            return new CreateShortenedEntryResponse(entry.Alias, entry.Url, entry.Creation, entry.Expiration);
        }
    }
}