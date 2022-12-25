namespace UrlShortener.Application.Persistence;

public interface IDatabaseConnectionProvider<TConnection>
{
    Task<TConnection> GetConnectionAsync(CancellationToken cancellationToken);
}