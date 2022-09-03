namespace URLShortener.Application.Interfaces;

public interface IAliasService
{
    Task<string> GetAvailableAliasAsync(CancellationToken cancellationToken = default);
}