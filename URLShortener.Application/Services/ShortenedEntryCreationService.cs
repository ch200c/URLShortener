using URLShortener.Domain;

namespace URLShortener.Application.Services;

public class ShortenedEntryCreationService : IShortenedEntryCreationService
{
    private readonly IAliasService _aliasService;

    public ShortenedEntryCreationService(IAliasService aliasService)
    {
        _aliasService = aliasService;
    }

    public async Task<ShortenedEntry> CreateAsync(
        CreateShortenedEntryRequest request, CancellationToken cancellationToken)
    {
        var alias = await request.Alias
            .IfNoneAsync(() => _aliasService.GetAvailableAliasAsync(cancellationToken));

        return new ShortenedEntry
        {
            Alias = alias,
            Url = request.Url,
            Creation = DateTime.UtcNow,
            Expiration = request.Expiration,
        };
    }
}