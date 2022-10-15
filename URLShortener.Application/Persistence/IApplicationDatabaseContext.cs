using Cassandra;

namespace URLShortener.Application.Persistence;

public interface IApplicationDatabaseContext
{
    Task<ICluster> GetClusterAsync(CancellationToken cancellationToken);
    Task<ISession> GetSessionAsync(CancellationToken cancellationToken);
}