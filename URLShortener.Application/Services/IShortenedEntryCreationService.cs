using UrlShortener.Domain;

namespace UrlShortener.Application.Services;

public interface IShortenedEntryCreationService
{
    Task<ShortenedEntry> CreateAsync(
        CreateShortenedEntryRequest request, CancellationToken cancellationToken);
}