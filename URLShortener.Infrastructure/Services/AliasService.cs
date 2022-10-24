using Confluent.Kafka;
using URLShortener.Application.Messaging;
using URLShortener.Application.Services;

namespace URLShortener.Infrastructure.Services;

public class AliasService : IAliasService
{
    private readonly IMessageConsumer<ConsumeResult<Null, string>> _messageConsumer;

    public AliasService(IMessageConsumer<ConsumeResult<Null, string>> messageConsumer)
    {
        _messageConsumer = messageConsumer;
    }

    public async Task<string> GetAvailableAliasAsync(CancellationToken cancellationToken)
    {
        var consumeResult = await _messageConsumer.ConsumeAsync(cancellationToken);
        return consumeResult.Message.Value;
    }
}