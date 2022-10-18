using URLShortener.Application.Services;

namespace URLShortener.Infrastructure.Services;

public class AliasService : IAliasService
{
    public Task<string> GetAvailableAliasAsync(CancellationToken cancellationToken)
    {
        // TODO:
        throw new NotImplementedException();
    }
}