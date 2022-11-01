using Cassandra;
using Cassandra.Mapping;
using LanguageExt;
using static LanguageExt.Prelude;
using URLShortener.Application.Persistence;

namespace URLShortener.Infrastructure.Persistence;

public sealed class CassandraConnectionProvider : IDatabaseConnectionProvider<ISession>, IAsyncDisposable, IDisposable
{
    private readonly SemaphoreSlim _clusterInitializationSemaphore = new(1, 1);
    private readonly SemaphoreSlim _sessionInitializationSemaphore = new(1, 1);
    private readonly IEnumerable<string> _contactPoints;
    private readonly int _port;
    private readonly string _keyspace;

    private Option<ICluster> _optionalCluster = Option<ICluster>.None;
    private Option<ISession> _optionalSession = Option<ISession>.None;

    public CassandraConnectionProvider(IEnumerable<string> contactPoints, int port, string keyspace)
    {
        _contactPoints = contactPoints;
        _port = port;
        _keyspace = keyspace;

        MappingConfiguration.Global.Define<ShortenedEntryMapping>();
    }

    public async Task<ISession> GetConnectionAsync(CancellationToken cancellationToken)
    {
        await _sessionInitializationSemaphore.WaitAsync(cancellationToken);
        ISession session;

        try
        {
            session = await _optionalSession.IfNoneAsync(async () =>
            {
                var cluster = await GetClusterAsync(cancellationToken);
                return await cluster.ConnectAsync(_keyspace);
            });

            _optionalSession = Some(session);
        }
        finally
        {
            _sessionInitializationSemaphore.Release();
        }

        return session;
    }

    private async Task<ICluster> GetClusterAsync(CancellationToken cancellationToken)
    {
        await _clusterInitializationSemaphore.WaitAsync(cancellationToken);
        ICluster cluster;

        try
        {
            cluster = _optionalCluster.IfNone(() =>
                Cluster
                    .Builder()
                    .AddContactPoints(_contactPoints)
                    .WithPort(_port)
                    .Build());

            _optionalCluster = Some(cluster);
        }
        finally
        {
            _clusterInitializationSemaphore.Release();
        }

        return cluster;
    }

    public async ValueTask DisposeAsync()
    {
        await _optionalCluster.IfSomeAsync(cluster =>
            cluster.ShutdownAsync());
    }

    public void Dispose()
    {
        _optionalCluster.IfSome(cluster =>
            cluster.Shutdown());
    }
}