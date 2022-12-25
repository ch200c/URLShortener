using Confluent.Kafka;
using Generator.Application;
using Generator.Application.Messaging;
using Generator.Application.Services;

namespace Generator.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly int _aliasLength;
    private readonly char[] _allowedCharacters;
    private readonly TimeSpan _aliasGenerationInterval;
    private readonly int _aliasGenerationCount;
    private readonly IAliasGenerator _aliasGenerator;
    private readonly IMessageProducer<Message<Null, string>> _messageProducer;

    public Worker(
        ILogger<Worker> logger,
        int aliasLength,
        char[] allowedCharacters,
        TimeSpan aliasGenerationInterval,
        int aliasGenerationCount,
        IAliasGenerator aliasGenerator,
        IMessageProducer<Message<Null, string>> messageProducer)
    {
        _logger = logger;
        _aliasLength = aliasLength;
        _allowedCharacters = allowedCharacters;
        _aliasGenerationInterval = aliasGenerationInterval;
        _aliasGenerationCount = aliasGenerationCount;
        _aliasGenerator = aliasGenerator;
        _messageProducer = messageProducer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            for (var i = 0; i < _aliasGenerationCount; i++)
            {
                var alias = GenerateAlias();

                var message = new Message<Null, string>() { Value = alias };
                await _messageProducer.ProduceAsync(message, stoppingToken);

                _logger.LogInformation("Generated {Alias}", alias);
            }

            await _messageProducer.FlushAsync(stoppingToken);

            _logger.LogDebug("Sleeping for {TimeSpan}", _aliasGenerationInterval);
            await Task.Delay(_aliasGenerationInterval, stoppingToken);
        }
    }

    private string GenerateAlias()
    {
        var request = new GenerateAliasRequest(_aliasLength, _allowedCharacters);
        return _aliasGenerator.Generate(request);
    }
}