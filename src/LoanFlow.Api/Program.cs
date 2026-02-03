using LoanFlow.Configuration;
using LoanFlow.Domain.Entities;
using LoanFlow.Infrastructure;
using LoanFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var databaseSettings = builder.Services.AddDatabaseSettings(builder.Configuration);
builder.Services.AddRabbitMQSettings(builder.Configuration);

builder.Services.AddInfrastructure(databaseSettings);
builder.Services.AddScoped<DbSeeder>();
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

        var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
        await seeder.SeedAsync();
    }
}

app.UseHttpsRedirection();

app.MapGet("/api/loans", async (LoanFlowDbContext db) =>
    await db.Loans.Where(l => !l.Deleted).ToListAsync());

app.MapGet("/api/loans/{id:guid}", async (Guid id, LoanFlowDbContext db) =>
    await db.Loans.FirstOrDefaultAsync(l => l.Id == id && !l.Deleted)
        is { } loan ? Results.Ok(loan) : Results.NotFound());

app.MapPost("/api/applicants", async (Applicant applicant, LoanFlowDbContext db) =>
{
    db.Applicants.Add(applicant);
    await db.SaveChangesAsync();
    return Results.Created($"/api/applicants/{applicant.Id}", applicant);
});

app.MapGet("/api/applicants/by-email/{email}", async (string email, LoanFlowDbContext db) =>
    await db.Applicants.FirstOrDefaultAsync(a => a.Email == email && !a.Deleted)
        is { } applicant ? Results.Ok(applicant) : Results.NotFound());

app.MapGet("/api/applicants/exists", async (string email, LoanFlowDbContext db) =>
    Results.Ok(new { exists = await db.Applicants.AnyAsync(a => a.Email == email && !a.Deleted) }));

app.Run();
