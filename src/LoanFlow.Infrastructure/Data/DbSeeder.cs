using LoanFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoanFlow.Infrastructure.Data;

public class DbSeeder
{
    private readonly LoanFlowDbContext _dbContext;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(LoanFlowDbContext dbContext, ILogger<DbSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _dbContext.Loans.AnyAsync())
        {
            _logger.LogInformation("Database already seeded, skipping");
            return;
        }

        _logger.LogInformation("Seeding database with test data...");

        var loans = new List<Loan>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BorrowerName = "John Smith",
                Amount = 25000m,
                InterestRate = 5.5m,
                TermMonths = 36,
                Status = LoanStatus.Active
            },
            new()
            {
                Id = Guid.NewGuid(),
                BorrowerName = "Sarah Johnson",
                Amount = 150000m,
                InterestRate = 4.25m,
                TermMonths = 360,
                Status = LoanStatus.Approved
            },
            new()
            {
                Id = Guid.NewGuid(),
                BorrowerName = "Mike Wilson",
                Amount = 10000m,
                InterestRate = 7.0m,
                TermMonths = 24,
                Status = LoanStatus.Pending
            },
            new()
            {
                Id = Guid.NewGuid(),
                BorrowerName = "Emily Davis",
                Amount = 50000m,
                InterestRate = 6.0m,
                TermMonths = 60,
                Status = LoanStatus.Completed
            },
            new()
            {
                Id = Guid.NewGuid(),
                BorrowerName = "Robert Brown",
                Amount = 75000m,
                InterestRate = 5.0m,
                TermMonths = 48,
                Status = LoanStatus.Rejected
            }
        };

        _dbContext.Loans.AddRange(loans);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Database seeded with {Count} loans", loans.Count);
    }
}
