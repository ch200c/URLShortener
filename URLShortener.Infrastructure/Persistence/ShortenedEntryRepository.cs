using Cassandra.Mapping;
using LanguageExt;
using URLShortener.Application;
using URLShortener.Application.Persistence;
using URLShortener.Domain;

namespace URLShortener.Infrastructure.Persistence;

public class ShortenedEntryRepository : IShortenedEntryRepository
{
    private readonly IApplicationDatabaseContext _databaseContext;

    public ShortenedEntryRepository(IApplicationDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<Option<ShortenedEntry>> GetAsync(string alias, CancellationToken cancellationToken = default)
    {
        var session = await _databaseContext.GetSessionAsync(cancellationToken);
        var mapper = new Mapper(session);

        return await mapper.SingleOrDefaultAsync<ShortenedEntry>("SELECT * FROM shortened_entries WHERE alias=?", alias);
    }

    public async Task<Option<ShortenedEntry>> CreateAsync(
        CreateShortenedEntryWithAliasRequest request, CancellationToken cancellationToken = default)
    {
        var entry = new ShortenedEntry()
        {
            Alias = request.Alias,
            Url = request.Url,
            Creation = DateTime.UtcNow,
            Expiration = request.Expiration
        };

        var session = await _databaseContext.GetSessionAsync(cancellationToken);
        var mapper = new Mapper(session);

        var appliedInfo = await mapper.InsertIfNotExistsAsync(entry);

        return appliedInfo.Applied ? entry : Option<ShortenedEntry>.None;
    }
}