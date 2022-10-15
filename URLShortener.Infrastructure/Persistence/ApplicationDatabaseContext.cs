using Cassandra;
using Cassandra.Mapping;
using URLShortener.Application.Mappings;
using URLShortener.Application.Persistence;

namespace URLShortener.Infrastructure.Persistence;

public sealed class ApplicationDatabaseContext : IApplicationDatabaseContext, IAsyncDisposable, IDisposable
{
    private readonly SemaphoreSlim _clusterInitializationSemaphore = new(1, 1);
    private readonly SemaphoreSlim _sessionInitializationSemaphore = new(1, 1);
    private readonly IEnumerable<string> _contactPoints;
    private readonly int _port;
    private readonly string _keyspace;
    private ICluster? _cluster;
    private ISession? _session;

    public ApplicationDatabaseContext(IEnumerable<string> contactPoints, int port, string keyspace)
    {
        _contactPoints = contactPoints;
        _port = port;
        _keyspace = keyspace;

        MappingConfiguration.Global.Define<ShortenedEntryMapping>();
    }

    public Task<ICluster> GetClusterAsync(CancellationToken cancellationToken = default)
    {
        _clusterInitializationSemaphore.WaitAsync(cancellationToken);

        try
        {
            _cluster ??= Cluster
                .Builder()
                .AddContactPoints(_contactPoints)
                .WithPort(_port)
                .Build();
        }
        finally
        {
            _clusterInitializationSemaphore.Release();
        }

        return Task.FromResult(_cluster);
    }

    public async Task<ISession> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        await _sessionInitializationSemaphore.WaitAsync(cancellationToken);

        try
        {
            if (_session == null)
            {
                var cluster = await GetClusterAsync(cancellationToken);
                _session = await cluster.ConnectAsync(_keyspace);
            }
        }
        finally
        {
            _sessionInitializationSemaphore.Release();
        }

        return _session;
    }

    public async ValueTask DisposeAsync()
    {
        if (_cluster == null)
        {
            return;
        }

        await _cluster.ShutdownAsync();
    }

    public void Dispose()
    {
        if (_cluster == null)
        {
            return;
        }

        _cluster.Shutdown();
    }
}