using URLShortener.Application.Interfaces;

namespace URLShortener.Infrastructure.Services;

public class AliasService : IAliasService
{
    public Task<string> GetAvailableAliasAsync(CancellationToken cancellationToken = default)
    {
        // TODO:
        throw new NotImplementedException();
    }
}