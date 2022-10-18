﻿using Cassandra.Mapping;
using LanguageExt;
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

    public async Task<Option<ShortenedEntry>> GetByAliasAsync(string alias, CancellationToken cancellationToken)
    {
        var session = await _databaseContext.GetSessionAsync(cancellationToken);
        var mapper = new Mapper(session);

        return await mapper.SingleOrDefaultAsync<ShortenedEntry>("SELECT * FROM shortened_entries WHERE alias=?", alias);
    }

    public async Task<Option<ShortenedEntry>> CreateAsync(
        ShortenedEntry shortenedEntry, CancellationToken cancellationToken)
    {
        var session = await _databaseContext.GetSessionAsync(cancellationToken);
        var mapper = new Mapper(session);

        var appliedInfo = await mapper.InsertIfNotExistsAsync(shortenedEntry);

        return appliedInfo.Applied ? shortenedEntry : Option<ShortenedEntry>.None;
    }
}