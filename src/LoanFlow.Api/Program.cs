using LoanFlow.Api.Models;
using LoanFlow.Configuration;
using LoanFlow.Contracts.Events;
using LoanFlow.Domain.Entities;
using LoanFlow.Infrastructure;
using LoanFlow.Infrastructure.Data;
using LoanFlow.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var databaseSettings = builder.Services.AddDatabaseSettings(builder.Configuration);
builder.Services.AddRabbitMQSettings(builder.Configuration);

builder.Services.AddInfrastructure(databaseSettings);
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();
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

app.MapPost("/api/applicants", async (ApplicantRequest request, LoanFlowDbContext db) =>
{
    var applicant = new Applicant
    {
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        DateOfBirth = request.DateOfBirth,
        MonthlyIncome = request.MonthlyIncome
    };

    db.Applicants.Add(applicant);
    await db.SaveChangesAsync();

    var response = new ApplicantResponse(
        applicant.Id,
        applicant.FirstName,
        applicant.LastName,
        applicant.Email,
        applicant.DateOfBirth,
        applicant.MonthlyIncome,
        applicant.Created);

    return Results.Created($"/api/applicants/{applicant.Id}", response);
});

app.MapGet("/api/applicants/by-email/{email}", async (string email, LoanFlowDbContext db) =>
{
    var applicant = await db.Applicants.FirstOrDefaultAsync(a => a.Email == email && !a.Deleted);
    if (applicant is null)
        return Results.NotFound();

    var response = new ApplicantResponse(
        applicant.Id,
        applicant.FirstName,
        applicant.LastName,
        applicant.Email,
        applicant.DateOfBirth,
        applicant.MonthlyIncome,
        applicant.Created);

    return Results.Ok(response);
});

app.MapGet("/api/applicants/exists", async (string email, LoanFlowDbContext db) =>
    Results.Ok(new { exists = await db.Applicants.AnyAsync(a => a.Email == email && !a.Deleted) }));

app.MapPost("/api/loan-applications", async (LoanApplicationRequest request, LoanFlowDbContext db, IMessagePublisher publisher) =>
{
    var applicant = await db.Applicants.FindAsync(request.ApplicantId);
    if (applicant is null)
        return Results.NotFound("Applicant not found");

    var application = new LoanApplication
    {
        ApplicantId = request.ApplicantId,
        Amount = request.Amount,
        Purpose = request.Purpose,
        Status = LoanApplicationStatus.Submitted,
        Submitted = DateTime.UtcNow,
        Applicant = applicant
    };

    db.LoanApplications.Add(application);
    await db.SaveChangesAsync();

    // Publish event for background processing
    var evt = new LoanSubmittedEvent(
        application.Id,
        application.ApplicantId,
        application.Amount,
        application.Purpose,
        application.Submitted);

    await publisher.PublishAsync(evt, "loan.submitted");

    var response = new LoanApplicationResponse(
        application.Id,
        application.ApplicantId,
        application.Amount,
        application.Purpose,
        application.Status,
        application.Submitted,
        application.Created);

    return Results.Created($"/api/loan-applications/{application.Id}", response);
});

// GET - List all loan applications
app.MapGet("/api/loan-applications", async (LoanFlowDbContext db) =>
{
    var applications = await db.LoanApplications
        .Where(la => !la.Deleted)
        .ToListAsync();

    return applications.Select(a => new LoanApplicationResponse(
        a.Id,
        a.ApplicantId,
        a.Amount,
        a.Purpose,
        a.Status,
        a.Submitted,
        a.Created));
});

// GET - Get loan application by ID
app.MapGet("/api/loan-applications/{id:guid}", async (Guid id, LoanFlowDbContext db) =>
{
    var application = await db.LoanApplications
        .FirstOrDefaultAsync(la => la.Id == id && !la.Deleted);

    if (application is null)
        return Results.NotFound();

    var response = new LoanApplicationResponse(
        application.Id,
        application.ApplicantId,
        application.Amount,
        application.Purpose,
        application.Status,
        application.Submitted,
        application.Created);

    return Results.Ok(response);
});

app.Run();
