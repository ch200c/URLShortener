using Cassandra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using URLShortener.Application.Persistence;

namespace URLShortener.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCassandra(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        return services.AddSingleton<IDatabaseConnectionProvider<ISession>, CassandraConnectionProvider>(serviceProvider =>
        {
            var contactPoints = configurationSection
                .GetSection("ContactPoints")
                .GetChildren()
                .Select(section => section.Value);

            var port = configurationSection.GetValue<int>("Port");
            var keyspace = configurationSection.GetValue<string>("Keyspace");

            return new CassandraConnectionProvider(contactPoints, port, keyspace);
        });
    }
}