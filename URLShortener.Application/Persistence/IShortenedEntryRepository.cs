using LanguageExt;
using URLShortener.Domain;

namespace URLShortener.Application.Persistence;

public interface IShortenedEntryRepository
{
    Task<Option<ShortenedEntry>> GetAsync(
        string alias, CancellationToken cancellationToken = default);

    Task<Option<ShortenedEntry>> CreateAsync(
        CreateShortenedEntryWithAliasRequest request, CancellationToken cancellationToken = default);
}