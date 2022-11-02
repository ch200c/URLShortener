using LanguageExt;
using URLShortener.Domain;

namespace URLShortener.Application.Services;

public interface IShortenedEntryService
{
    Task<Option<ShortenedEntry>> GetByAliasAsync(string alias, CancellationToken cancellationToken);
    Task<Option<ShortenedEntry>> CreateAsync(CreateShortenedEntryRequest request, CancellationToken cancellationToken);
}