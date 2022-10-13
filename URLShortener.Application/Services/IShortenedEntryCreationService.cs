using LanguageExt;
using URLShortener.Domain;

namespace URLShortener.Application.Services;

public interface IShortenedEntryCreationService
{
    Task<Option<ShortenedEntry>> CreateAsync(
        CreateShortenedEntryRequest request, CancellationToken cancellationToken = default);
}