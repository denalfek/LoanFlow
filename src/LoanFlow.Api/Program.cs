using LoanFlow.Configuration;
using LoanFlow.Infrastructure;
using LoanFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var databaseSettings = builder.Services.AddDatabaseSettings(builder.Configuration);
builder.Services.AddRabbitMQSettings(builder.Configuration);

builder.Services.AddInfrastructure(databaseSettings);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    if (databaseSettings.AutoMigrate)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LoanFlowDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}

app.UseHttpsRedirection();

app.Run();
