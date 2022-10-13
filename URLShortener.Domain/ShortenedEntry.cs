namespace URLShortener.Domain;
// todo option<> and better than  null!
public class ShortenedEntry
{
    public string Alias { get; set; }
    public string Url { get; set; }
    public DateTime Creation { get; set; }
    public DateTime Expiration { get; set; }
}