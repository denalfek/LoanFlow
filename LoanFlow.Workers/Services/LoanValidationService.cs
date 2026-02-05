using LoanFlow.Domain.Entities;
using LoanFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoanFlow.Workers.Services;

public record ValidationResult(bool IsValid, string[] Errors);

public class LoanValidationService
{
    private readonly LoanFlowDbContext _dbContext;
    private readonly ILogger<LoanValidationService> _logger;

    private const decimal MinAmount = 1_000m;
    private const decimal MaxAmount = 500_000m;
    private const int MinAge = 18;
    private const int MaxAge = 70;

    public LoanValidationService(LoanFlowDbContext dbContext, ILogger<LoanValidationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateAsync(Guid loanApplicationId, CancellationToken cancellationToken = default)
    {
        var application = await _dbContext.LoanApplications
            .Include(la => la.Applicant)
            .FirstOrDefaultAsync(la => la.Id == loanApplicationId, cancellationToken);

        if (application is null)
        {
            _logger.LogWarning("Loan application {LoanApplicationId} not found", loanApplicationId);
            return new ValidationResult(false, ["Loan application not found"]);
        }

        var errors = new List<string>();

        // Validate amount
        if (application.Amount < MinAmount)
            errors.Add($"Amount must be at least ${MinAmount:N0}");

        if (application.Amount > MaxAmount)
            errors.Add($"Amount cannot exceed ${MaxAmount:N0}");

        // Validate purpose
        if (string.IsNullOrWhiteSpace(application.Purpose))
            errors.Add("Purpose is required");

        // Validate applicant age
        var age = CalculateAge(application.Applicant.DateOfBirth);
        if (age < MinAge)
            errors.Add($"Applicant must be at least {MinAge} years old");

        if (age > MaxAge)
            errors.Add($"Applicant cannot be older than {MaxAge} years");

        var isValid = errors.Count == 0;

        _logger.LogInformation(
            "Validation result for application {LoanApplicationId}: {IsValid}, Errors: {Errors}",
            loanApplicationId, isValid, string.Join(", ", errors));

        return new ValidationResult(isValid, errors.ToArray());
    }

    public async Task UpdateStatusAsync(Guid loanApplicationId, LoanApplicationStatus status, CancellationToken cancellationToken = default)
    {
        var application = await _dbContext.LoanApplications
            .FirstOrDefaultAsync(la => la.Id == loanApplicationId, cancellationToken);

        if (application is not null)
        {
            application.Status = status;
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated application {LoanApplicationId} status to {Status}", loanApplicationId, status);
        }
    }

    private static int CalculateAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Year;

        if (birthDate > today.AddYears(-age))
            age--;

        return age;
    }
}
