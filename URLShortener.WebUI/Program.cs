using URLShortener.Application.Services;
using URLShortener.Infrastructure.Services;
using URLShortener.Infrastructure.Persistence;
using URLShortener.Application.Persistence;
using URLShortener.Infrastructure.Messaging;
using Confluent.Kafka;
using URLShortener.Application.Messaging;

var builder = WebApplication.CreateBuilder(args);

const string corsPolicyName = "custom";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        // TODO: Test out in non-dev env - is this required?
        var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';') ?? Array.Empty<string>();
        policy.WithOrigins(urls).WithMethods("GET").AllowAnyHeader();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCassandra(builder.Configuration.GetSection("ConnectionStrings:Cassandra"));

builder.Services.AddSingleton<KafkaClientHandle>(_ =>
{
    var producerConfig = new ProducerConfig();
    builder.Configuration.GetSection("Kafka:ProducerSettings")
        .Bind(producerConfig);

    return new KafkaClientHandle(producerConfig);
});

builder.Services.AddSingleton<IMessageProducer<Message<string, string>>, KafkaProducer<string, string>>(serviceProvider =>
{
    var kafkaClientHandle = serviceProvider.GetRequiredService<KafkaClientHandle>();
    var topic = builder.Configuration.GetValue<string>("Kafka:AliasTopic");
    return new KafkaProducer<string, string>(kafkaClientHandle, topic);
});

builder.Services.AddTransient<IShortenedEntryRepository, ShortenedEntryRepository>();
builder.Services.AddTransient<IAliasService, AliasService>();
builder.Services.AddTransient<IShortenedEntryCreationService, ShortenedEntryCreationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();