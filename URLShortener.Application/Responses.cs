namespace URLShortener.Application;
// todo option
public record class CreateShortenedEntryResponse(
    string Alias, string Url, DateTime Creation, DateTime Expiration);