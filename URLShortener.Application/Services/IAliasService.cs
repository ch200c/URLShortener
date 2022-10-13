namespace URLShortener.Application.Services;

public interface IAliasService
{
    Task<string> GetAvailableAliasAsync(CancellationToken cancellationToken = default);
}