using LanguageExt;
using UrlShortener.Application.Persistence;
using UrlShortener.Domain;

namespace UrlShortener.Application.Services;

public class ShortenedEntryService : IShortenedEntryService
{
    private readonly IShortenedEntryRepository _shortenedEntryRepository;
    private readonly IShortenedEntryCreationService _shortenedEntryCreationService;

    public ShortenedEntryService(
        IShortenedEntryRepository shortenedEntryRepository,
        IShortenedEntryCreationService shortenedEntryCreationService)
    {
        _shortenedEntryRepository = shortenedEntryRepository;
        _shortenedEntryCreationService = shortenedEntryCreationService;
    }

    public async Task<Option<ShortenedEntry>> CreateAsync(CreateShortenedEntryRequest request, CancellationToken cancellationToken)
    {
        var shortenedEntry = await _shortenedEntryCreationService.CreateAsync(request, cancellationToken);
        return await _shortenedEntryRepository.CreateAsync(shortenedEntry, cancellationToken);
    }

    public Task<Option<ShortenedEntry>> GetByAliasAsync(string alias, CancellationToken cancellationToken)
    {
        return _shortenedEntryRepository.GetByAliasAsync(alias, cancellationToken);
    }
}
