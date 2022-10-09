namespace URLShortener.Application;
// todo option
public record class CreateShortenedEntryResponse(
    string Alias, string Url, Guid? UserId, DateTime Creation, DateTime Expiration);