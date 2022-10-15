using Cassandra;

namespace URLShortener.Application.Persistence;

public interface IApplicationDatabaseContext
{
    Task<ICluster> GetClusterAsync(CancellationToken cancellationToken = default);
    Task<ISession> GetSessionAsync(CancellationToken cancellationToken = default);
}