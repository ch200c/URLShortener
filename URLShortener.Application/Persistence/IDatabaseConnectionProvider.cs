namespace URLShortener.Application.Persistence;

public interface IDatabaseConnectionProvider<TConnection>
{
    Task<TConnection> GetConnectionAsync(CancellationToken cancellationToken);
}