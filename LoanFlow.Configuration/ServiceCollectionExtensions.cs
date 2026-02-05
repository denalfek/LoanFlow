using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LoanFlow.Configuration;

public static class ServiceCollectionExtensions
{
    public static DatabaseSettings AddDatabaseSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(DatabaseSettings.SectionName);
        services.Configure<DatabaseSettings>(section);

        var settings = section.Get<DatabaseSettings>() ?? new DatabaseSettings();
        return settings;
    }

    public static RabbitMQSettings AddRabbitMQSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(RabbitMQSettings.SectionName);
        services.Configure<RabbitMQSettings>(section);

        var settings = section.Get<RabbitMQSettings>() ?? new RabbitMQSettings();
        return settings;
    }
}
