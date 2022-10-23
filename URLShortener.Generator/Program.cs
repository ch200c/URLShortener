using URLShortener.Application.Persistence;
using URLShortener.Application.Services;
using URLShortener.Generator;
using URLShortener.Infrastructure.Persistence;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddCassandra(context.Configuration.GetSection("Cassandra"));

        services.AddTransient<IAliasGenerator, AliasGenerator>();
        services.AddTransient<IShortenedEntryRepository, ShortenedEntryRepository>();

        services.AddHostedService(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Worker>>();
            var aliasLength = context.Configuration.GetValue<int>("AliasLength");
            var allowedCharacters = context.Configuration.GetValue<string>("AllowedCharacters").ToArray();
            var aliasGenerationInterval = context.Configuration.GetValue<TimeSpan>("AliasGenerationInterval");
            var aliasGenerationCount = context.Configuration.GetValue<int>("AliasGenerationCount");
            var aliasGenerator = serviceProvider.GetRequiredService<IAliasGenerator>();
            var shortenedEntryRepository = serviceProvider.GetRequiredService<IShortenedEntryRepository>();

            return new Worker(
                logger,
                aliasLength,
                allowedCharacters,
                aliasGenerationInterval,
                aliasGenerationCount,
                aliasGenerator,
                shortenedEntryRepository);
        });
    })
    .Build();

await host.RunAsync();
