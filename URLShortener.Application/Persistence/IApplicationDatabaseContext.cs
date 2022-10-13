using Cassandra;

namespace URLShortener.Application.Persistence;

public interface IApplicationDatabaseContext
{
    ICluster Cluster { get; }
    ISession Session { get; }
}