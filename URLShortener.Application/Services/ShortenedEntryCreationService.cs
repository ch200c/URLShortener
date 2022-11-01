using LanguageExt;
using URLShortener.Application.Persistence;
using URLShortener.Domain;

namespace URLShortener.Application.Services;

// TODO: Resilience policies
public class ShortenedEntryCreationService : IShortenedEntryCreationService
{
    private readonly IAliasService _aliasService;
    private readonly IShortenedEntryRepository _shortenedEntryRepository;

    public ShortenedEntryCreationService(
        IAliasService aliasService,
        IShortenedEntryRepository shortenedEntryRepository)
    {
        _aliasService = aliasService;
        _shortenedEntryRepository = shortenedEntryRepository;
    }

    public async Task<Option<ShortenedEntry>> CreateAsync(
        CreateShortenedEntryRequest request, CancellationToken cancellationToken)
    {
        var alias = await request.Alias.IfNoneAsync(() => 
            _aliasService.GetAvailableAliasAsync(cancellationToken));

        var shortenedEntry = new ShortenedEntry
        {
            Alias = alias,
            Url = request.Url,
            Creation = DateTime.UtcNow,
            Expiration = request.Expiration,
        };

        return await _shortenedEntryRepository.CreateAsync(shortenedEntry, cancellationToken);
    }
}