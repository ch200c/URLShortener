using Cassandra;

namespace URLShortener.Application.Interfaces;

public interface IApplicationDatabaseContext
{
    ICluster Cluster { get; }
    ISession Session { get; }
}