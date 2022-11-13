using URLShortener.Application.Services;
using URLShortener.Infrastructure.Services;
using URLShortener.Infrastructure.Persistence;
using URLShortener.Application.Persistence;
using URLShortener.Infrastructure.Messaging;
using Confluent.Kafka;
using URLShortener.Application.Messaging;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Application.Converters;
using URLShortener.API.SchemaFilters;

var builder = WebApplication.CreateBuilder(args);

const string corsPolicyName = "custom";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';') ?? Array.Empty<string>();
        policy.WithOrigins(urls).WithMethods("GET").AllowAnyHeader();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<CreateShortenedEntryRequestSchemaFilter>();
});

builder.Services.Configure<JsonOptions>(options =>
    options.JsonSerializerOptions.Converters.Add(new OptionOfStringJsonConverter()));

builder.Services.AddCassandra(builder.Configuration.GetSection("Cassandra"));

builder.Services.AddSingleton<IMessageConsumer<ConsumeResult<Null, string>>, KafkaConsumer>(_ =>
{
    var consumerConfig = new ConsumerConfig();
    builder.Configuration
        .GetSection("Kafka:ConsumerSettings")
        .Bind(consumerConfig);

    var topic = builder.Configuration.GetValue<string>("Kafka:AliasCandidatesTopic");

    return new KafkaConsumer(consumerConfig, topic);
});

builder.Services.AddTransient<IShortenedEntryRepository, ShortenedEntryRepository>();
builder.Services.AddTransient<IAliasService, AliasService>();
builder.Services.AddTransient<IShortenedEntryCreationService, ShortenedEntryCreationService>();
builder.Services.AddTransient<IShortenedEntryService, ShortenedEntryService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();