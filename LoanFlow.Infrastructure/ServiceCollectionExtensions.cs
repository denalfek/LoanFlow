using LoanFlow.Configuration;
using LoanFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LoanFlow.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        DatabaseSettings settings)
    {
        services.AddDbContext<LoanFlowDbContext>(options =>
        {
            options.UseNpgsql(settings.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(settings.CommandTimeout);
            });

            if (settings.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}
