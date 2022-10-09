using LanguageExt;
using URLShortener.Application;
using URLShortener.Application.Interfaces;
using URLShortener.Domain;

namespace URLShortener.Infrastructure.Services;

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
        CreateShortenedEntryRequest request, CancellationToken cancellationToken = default)
    {
        var isAliasGenerationRequired = request.Alias == null;

        var alias = isAliasGenerationRequired
            ? await _aliasService.GetAvailableAliasAsync(cancellationToken)
            : request.Alias;

        var requestWithAlias = new CreateShortenedEntryWithAliasRequest(alias, request.Url, request.UserId, request.Expiration);

        return await _shortenedEntryRepository.CreateAsync(requestWithAlias, cancellationToken);
    }
}