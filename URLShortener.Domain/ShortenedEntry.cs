namespace URLShortener.Domain;

public class ShortenedEntry
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Alias { get; set; }
    public string Url { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Guid? UserId { get; set; }
    public DateTime Creation { get; set; }
    public DateTime Expiration { get; set; }
}