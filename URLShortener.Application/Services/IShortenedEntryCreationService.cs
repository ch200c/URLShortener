using URLShortener.Domain;

namespace URLShortener.Application.Services;

public interface IShortenedEntryCreationService
{
    Task<ShortenedEntry> CreateAsync(
        CreateShortenedEntryRequest request, CancellationToken cancellationToken);
}