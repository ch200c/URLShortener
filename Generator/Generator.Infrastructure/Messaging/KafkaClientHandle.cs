using Confluent.Kafka;

namespace Generator.Infrastructure.Messaging;

public sealed class KafkaClientHandle : IDisposable
{
    public Handle Handle { get => _producer.Handle; }

    private readonly IProducer<byte[], byte[]> _producer;

    public KafkaClientHandle(ProducerConfig producerConfig)
    {
        _producer = new ProducerBuilder<byte[], byte[]>(producerConfig)
            .Build();
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}
