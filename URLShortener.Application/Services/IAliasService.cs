namespace UrlShortener.Application.Services;

public interface IAliasService
{
    Task<string> GetAvailableAliasAsync(CancellationToken cancellationToken);
}