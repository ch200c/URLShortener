using Confluent.Kafka;
using URLShortener.Application;
using URLShortener.Application.Messaging;
using URLShortener.Application.Persistence;
using URLShortener.Application.Services;

namespace URLShortener.Generator;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly int _aliasLength;
    private readonly char[] _allowedCharacters;
    private readonly TimeSpan _aliasGenerationInterval;
    private readonly int _aliasGenerationCount;
    private readonly IAliasGenerator _aliasGenerator;
    private readonly IShortenedEntryRepository _shortenedEntryRepository;
    private readonly IMessageProducer<Message<Null, string>> _messageProducer;

    public Worker(
        ILogger<Worker> logger,
        int aliasLength,
        char[] allowedCharacters,
        TimeSpan aliasGenerationInterval,
        int aliasGenerationCount,
        IAliasGenerator aliasGenerator,
        IShortenedEntryRepository shortenedEntryRepository,
        IMessageProducer<Message<Null, string>> messageProducer)
    {
        _logger = logger;
        _aliasLength = aliasLength;
        _allowedCharacters = allowedCharacters;
        _aliasGenerationInterval = aliasGenerationInterval;
        _aliasGenerationCount = aliasGenerationCount;
        _aliasGenerator = aliasGenerator;
        _shortenedEntryRepository = shortenedEntryRepository;
        _messageProducer = messageProducer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            for (var i = 0; i < _aliasGenerationCount; i++)
            {
                var alias = await GenerateAliasAsync(stoppingToken);

                var message = new Message<Null, string>() { Value = alias };
                await _messageProducer.ProduceAsync(message, stoppingToken);

                _logger.LogInformation("Generated {Alias}", alias);
            }

            await _messageProducer.FlushAsync(stoppingToken);

            _logger.LogDebug("Sleeping for {TimeSpan}", _aliasGenerationInterval);
            await Task.Delay(_aliasGenerationInterval, stoppingToken);
        }
    }

    private async Task<string> GenerateAliasAsync(CancellationToken cancellationToken)
    {
        var request = new GenerateAliasRequest(_aliasLength, _allowedCharacters);
        var aliasCandidate = _aliasGenerator.Generate(request);

        var isGenerating = true;

        while (isGenerating)
        {
            var optionalEntry = await _shortenedEntryRepository.GetByAliasAsync(aliasCandidate, cancellationToken);

            optionalEntry
                .Some(entry =>
                {
                    aliasCandidate = _aliasGenerator.Generate(request);
                })
                .None(() =>
                {
                    isGenerating = false;
                });
        }

        return aliasCandidate;
    }
}