using Cassandra;
using Cassandra.Mapping;
using LanguageExt;
using UrlShortener.Application.Persistence;
using UrlShortener.Domain;

namespace UrlShortener.Infrastructure.Persistence;

public class ShortenedEntryRepository : IShortenedEntryRepository
{
    private readonly IDatabaseConnectionProvider<ISession> _databaseConnectionProvider;

    public ShortenedEntryRepository(IDatabaseConnectionProvider<ISession> databaseConnectionProvider)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public async Task<Option<ShortenedEntry>> GetByAliasAsync(string alias, CancellationToken cancellationToken)
    {
        var session = await _databaseConnectionProvider.GetConnectionAsync(cancellationToken);
        var mapper = new Mapper(session);

        return await mapper.SingleOrDefaultAsync<ShortenedEntry>("SELECT * FROM shortened_entries WHERE alias=?", alias);
    }

    public async Task<Option<ShortenedEntry>> CreateAsync(
        ShortenedEntry shortenedEntry, CancellationToken cancellationToken)
    {
        var session = await _databaseConnectionProvider.GetConnectionAsync(cancellationToken);
        var mapper = new Mapper(session);

        var appliedInfo = await mapper.InsertIfNotExistsAsync(shortenedEntry);

        return appliedInfo.Applied
            ? shortenedEntry
            : Option<ShortenedEntry>.None;
    }
}