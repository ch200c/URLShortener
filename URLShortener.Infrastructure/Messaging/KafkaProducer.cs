using Confluent.Kafka;
using URLShortener.Application.Messaging;

namespace URLShortener.Infrastructure.Messaging;

public sealed class KafkaProducer<TKey, TValue> : IMessageProducer<Message<TKey, TValue>>, IDisposable
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly string _topic;

    public KafkaProducer(KafkaClientHandle kafkaClientHandle, string topic)
    {
        _producer = new DependentProducerBuilder<TKey, TValue>(kafkaClientHandle.Handle)
            .Build();
        _topic = topic;
    }

    public Task ProduceAsync(Message<TKey, TValue> message, CancellationToken cancellationToken)
    {
        return _producer.ProduceAsync(_topic, message, cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken)
    {
        _producer.Flush(cancellationToken);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}
