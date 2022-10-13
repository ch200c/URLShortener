using URLShortener.Application.Services;
using URLShortener.Infrastructure.Services;
using URLShortener.Infrastructure.Persistence;
using URLShortener.Application.Persistence;

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

builder.Services.AddSingleton<IApplicationDatabaseContext, ApplicationDatabaseContext>(serviceProvider =>
{
    var contactPoints = builder.Configuration
        .GetSection("ConnectionStrings:Cassandra:ContactPoints")
        .GetChildren()
        .Select(section => section.Value);

    var port = builder.Configuration.GetValue<int>("ConnectionStrings:Cassandra:Port");
    var keyspace = builder.Configuration.GetValue<string>("ConnectionStrings:Cassandra:Keyspace");

    return new ApplicationDatabaseContext(contactPoints, port, keyspace);
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