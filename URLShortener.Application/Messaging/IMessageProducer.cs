namespace URLShortener.Application.Messaging;

public interface IMessageProducer<in TMessage>
{
    Task ProduceAsync(TMessage message, CancellationToken cancellationToken);
    Task FlushAsync(CancellationToken cancellationToken);
}
