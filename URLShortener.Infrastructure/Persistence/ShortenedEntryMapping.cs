using UrlShortener.Domain;

namespace UrlShortener.Infrastructure.Persistence;

public class ShortenedEntryMapping : Cassandra.Mapping.Mappings
{
    public ShortenedEntryMapping()
    {
        For<ShortenedEntry>()
            .TableName("shortened_entries")
            .PartitionKey(entry => entry.Alias)
            .Column(entry => entry.Alias, columnMap => columnMap.WithName("alias"))
            .Column(entry => entry.Url, columnMap => columnMap.WithName("url"))
            .Column(entry => entry.Creation, columnMap => columnMap.WithName("creation"))
            .Column(entry => entry.Expiration, columnMap => columnMap.WithName("expiration"));
    }
}