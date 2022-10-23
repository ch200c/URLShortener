namespace URLShortener.Application.Persistence;

public interface IApplicationDatabaseContext<TSession>
{
    Task<TSession> GetSessionAsync(CancellationToken cancellationToken);
}