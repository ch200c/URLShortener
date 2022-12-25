using Confluent.Kafka;
using Generator.Application.Messaging;
using Generator.Application.Services;
using Generator.Worker;
using Generator.Infrastructure.Messaging;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<KafkaClientHandle>(_ =>
        {
            var producerConfig = new ProducerConfig();
            context.Configuration.GetSection("Kafka:ProducerSettings")
                .Bind(producerConfig);

            return new KafkaClientHandle(producerConfig);
        });

        services.AddSingleton<IMessageProducer<Message<Null, string>>, KafkaProducer<Null, string>>(serviceProvider =>
        {
            var kafkaClientHandle = serviceProvider.GetRequiredService<KafkaClientHandle>();
            var topic = context.Configuration.GetValue<string>("Kafka:AliasCandidatesTopic");

            return new KafkaProducer<Null, string>(kafkaClientHandle, topic);
        });

        services.AddTransient<IAliasGenerator, AliasGenerator>();

        services.AddHostedService(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Worker>>();
            var aliasLength = context.Configuration.GetValue<int>("AliasLength");
            var allowedCharacters = context.Configuration.GetValue<string>("AllowedCharacters").ToArray();
            var aliasGenerationInterval = context.Configuration.GetValue<TimeSpan>("AliasGenerationInterval");
            var aliasGenerationCount = context.Configuration.GetValue<int>("AliasGenerationCount");
            var aliasGenerator = serviceProvider.GetRequiredService<IAliasGenerator>();
            var messageProducer = serviceProvider.GetRequiredService<IMessageProducer<Message<Null, string>>>();

            return new Worker(
                logger,
                aliasLength,
                allowedCharacters,
                aliasGenerationInterval,
                aliasGenerationCount,
                aliasGenerator,
                messageProducer);
        });
    })
    .Build();

await host.RunAsync();
