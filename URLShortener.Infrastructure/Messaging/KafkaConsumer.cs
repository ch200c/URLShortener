using Confluent.Kafka;
using URLShortener.Application.Messaging;

namespace URLShortener.Infrastructure.Messaging;

public sealed class KafkaConsumer : IMessageConsumer<ConsumeResult<Null, string>>, IDisposable
{
    private readonly IConsumer<Null, string> _consumer;

    public KafkaConsumer(ConsumerConfig consumerConfig, string topic)
    {
        _consumer = new ConsumerBuilder<Null, string>(consumerConfig)
            .Build();

        _consumer.Subscribe(topic);
    }

    public Task<ConsumeResult<Null, string>> ConsumeAsync(CancellationToken cancellationToken)
    {
        var consumeResult = _consumer.Consume(cancellationToken);
        return Task.FromResult(consumeResult);
    }

    public void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
    }
}
