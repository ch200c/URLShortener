namespace URLShortener.Application;

public record class CreateShortenedEntryResponse(
    string Alias, string Url, Guid? UserId, DateTime Creation, DateTime Expiration);