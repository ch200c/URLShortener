using LanguageExt;
using UrlShortener.Domain;

namespace UrlShortener.Application.Services;

public interface IShortenedEntryService
{
    Task<Option<ShortenedEntry>> GetByAliasAsync(string alias, CancellationToken cancellationToken);
    Task<Option<ShortenedEntry>> CreateAsync(CreateShortenedEntryRequest request, CancellationToken cancellationToken);
}