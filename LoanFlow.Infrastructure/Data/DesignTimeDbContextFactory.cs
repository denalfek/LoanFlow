using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LoanFlow.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LoanFlowDbContext>
{
    public LoanFlowDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString")
            ?? "Host=localhost;Database=loanflow;Username=loanuser;Password=loanpass";

        var optionsBuilder = new DbContextOptionsBuilder<LoanFlowDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        var context = new LoanFlowDbContext(optionsBuilder.Options);
        return context;
    }
}
