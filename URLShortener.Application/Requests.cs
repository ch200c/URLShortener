namespace URLShortener.Application;

public record class CreateShortenedEntryRequest(string? Alias, string Url, Guid? UserId, DateTime Expiration);
public record class CreateShortenedEntryWithAliasRequest(string Alias, string Url, Guid? UserId, DateTime Expiration);