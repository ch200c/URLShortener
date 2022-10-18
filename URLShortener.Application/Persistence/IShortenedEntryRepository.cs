using LanguageExt;
using URLShortener.Domain;

namespace URLShortener.Application.Persistence;

public interface IShortenedEntryRepository
{
    Task<Option<ShortenedEntry>> GetByAliasAsync(
        string alias, CancellationToken cancellationToken);

    Task<Option<ShortenedEntry>> CreateAsync(
        ShortenedEntry shortenedEntry, CancellationToken cancellationToken);
}