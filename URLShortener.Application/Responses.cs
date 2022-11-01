namespace URLShortener.Application;

public record class CreateShortenedEntryResponse(string Alias, string Url, DateTime Creation, DateTime Expiration);