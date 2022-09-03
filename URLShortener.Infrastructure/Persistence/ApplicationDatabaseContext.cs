using Cassandra;
using Cassandra.Mapping;
using URLShortener.Application.Interfaces;
using URLShortener.Application.Mappings;

namespace URLShortener.Infrastructure.Persistence;

public class ApplicationDatabaseContext : IApplicationDatabaseContext, IDisposable
{
    public ICluster Cluster { get; }
    public ISession Session { get; }

    private bool _disposed;

    public ApplicationDatabaseContext()
    {
        // TODO: read from config
        Cluster = Cassandra.Cluster
            .Builder()
            .AddContactPoints("127.0.0.1")
            .WithPort(9042)
            .Build();

        Session = Cluster.Connect("store");

        MappingConfiguration.Global.Define<ShortenedEntryMapping>();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Session.Dispose();
            Cluster.Dispose();
        }

        _disposed = true;
    }
}