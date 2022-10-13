namespace URLShortener.Application;
// todo option
public record class CreateShortenedEntryRequest(string? Alias, string Url, DateTime Expiration);
public record class CreateShortenedEntryWithAliasRequest(string Alias, string Url, DateTime Expiration);
public record class GenerateAliasRequest(int Length, char[] AllowedChars);