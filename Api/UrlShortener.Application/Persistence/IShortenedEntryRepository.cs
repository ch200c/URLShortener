using LanguageExt;
using UrlShortener.Domain;

namespace UrlShortener.Application.Persistence;

public interface IShortenedEntryRepository
{
    Task<Option<ShortenedEntry>> GetByAliasAsync(
        string alias, CancellationToken cancellationToken);

    Task<Option<ShortenedEntry>> CreateAsync(
        ShortenedEntry shortenedEntry, CancellationToken cancellationToken);
}