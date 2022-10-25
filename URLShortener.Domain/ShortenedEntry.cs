namespace URLShortener.Domain;

public class ShortenedEntry
{
    public string Alias { get; set; } = default!;
    public string Url { get; set; } = default!;
    public DateTime Creation { get; set; }
    public DateTime Expiration { get; set; }
}