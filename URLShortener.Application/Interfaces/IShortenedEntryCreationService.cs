using LanguageExt;
using URLShortener.Domain;

namespace URLShortener.Application.Interfaces;

public interface IShortenedEntryCreationService
{
    Task<Option<ShortenedEntry>> CreateAsync(
        CreateShortenedEntryRequest request, CancellationToken cancellationToken = default);
}