using URLShortener.Application;
using URLShortener.Application.Persistence;
using URLShortener.Application.Services;

namespace URLShortener.Generator
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly int _aliasLength;
        private readonly char[] _allowedCharacters;
        private readonly TimeSpan _aliasGenerationInterval;
        private readonly IAliasGenerator _aliasGenerator;
        private readonly IShortenedEntryRepository _shortenedEntryRepository;

        public Worker(
            ILogger<Worker> logger,
            int aliasLength,
            char[] allowedCharacters,
            TimeSpan aliasGenerationInterval,
            IAliasGenerator aliasGenerator,
            IShortenedEntryRepository shortenedEntryRepository)
        {
            _logger = logger;
            _aliasLength = aliasLength;
            _allowedCharacters = allowedCharacters;
            _aliasGenerationInterval = aliasGenerationInterval;
            _aliasGenerator = aliasGenerator;
            _shortenedEntryRepository = shortenedEntryRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var alias = await GenerateAliasAsync(stoppingToken);

                // Publish

                _logger.LogDebug("Sleeping for {TimeSpan}", _aliasGenerationInterval);
                await Task.Delay(_aliasGenerationInterval, stoppingToken);
            }
        }

        // TODO: Optimize in batches/cache, etc
        private async Task<string> GenerateAliasAsync(CancellationToken cancellationToken)
        {
            var request = new GenerateAliasRequest(_aliasLength, _allowedCharacters);
            var aliasCandidate = _aliasGenerator.Generate(request);

            var isGenerating = true;

            while (isGenerating)
            {
                var optionalEntry = await _shortenedEntryRepository.GetAsync(aliasCandidate, cancellationToken);

                optionalEntry.Match(
                    entry =>
                    {
                        aliasCandidate = _aliasGenerator.Generate(request);
                    },
                    () =>
                    {
                        isGenerating = false;
                    });
            }

            return aliasCandidate;
        }
    }
}