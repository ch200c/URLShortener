namespace URLShortener.Application.Messaging;

public interface IMessageConsumer<TMessage>
{
    public Task<TMessage> ConsumeAsync(CancellationToken cancellationToken);
}
