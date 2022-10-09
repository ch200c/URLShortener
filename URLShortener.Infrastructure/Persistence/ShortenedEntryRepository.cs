using Cassandra.Mapping;
using LanguageExt;
using URLShortener.Application;
using URLShortener.Application.Interfaces;
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
        var mapper = new Mapper(_databaseContext.Session);
        return await mapper.SingleOrDefaultAsync<ShortenedEntry>("SELECT * FROM shortened_entries WHERE alias=?", alias);
    }

    public async Task<Option<ShortenedEntry>> CreateAsync(
        CreateShortenedEntryWithAliasRequest request, CancellationToken cancellationToken = default)
    {
        var entry = new ShortenedEntry()
        {
            Alias = request.Alias,
            Url = request.Url,
            UserId = request.UserId,
            Creation = DateTime.UtcNow,
            Expiration = request.Expiration
        };
       
        var mapper = new Mapper(_databaseContext.Session);
        var appliedInfo = await mapper.InsertIfNotExistsAsync(entry);

        return appliedInfo.Applied ? entry : Option<ShortenedEntry>.None;
    }
}