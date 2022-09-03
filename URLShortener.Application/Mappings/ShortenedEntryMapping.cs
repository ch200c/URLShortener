using URLShortener.Domain;

namespace URLShortener.Application.Mappings;

public class ShortenedEntryMapping : Cassandra.Mapping.Mappings
{
    public ShortenedEntryMapping()
    {
        For<ShortenedEntry>()
            .TableName("shortened_entries")
            .PartitionKey(entry => entry.Alias)
            .Column(entry => entry.Alias, columnMap => columnMap.WithName("alias"))
            .Column(entry => entry.Url, columnMap => columnMap.WithName("url"))
            .Column(entry => entry.UserId, columnMap => columnMap.WithName("user_id"))
            .Column(entry => entry.Creation, columnMap => columnMap.WithName("creation"))
            .Column(entry => entry.Expiration, columnMap => columnMap.WithName("expiration"));
    }
}