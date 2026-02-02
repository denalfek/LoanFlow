using LoanFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.Infrastructure.Data;

public class LoanFlowDbContext : DbContext
{
    public LoanFlowDbContext(DbContextOptions<LoanFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Loan> Loans => Set<Loan>();

    public override int SaveChanges()
    {
        ApplyAuditFields();
        var result = base.SaveChanges();
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    private void ApplyAuditFields()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            var entity = entry.Entity;
            var entityType = entity.GetType();

            if (!IsBaseEntity(entityType))
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    entityType.GetProperty("Created")?.SetValue(entity, now);
                    break;
                case EntityState.Modified:
                    entityType.GetProperty("Updated")?.SetValue(entity, now);
                    break;
            }
        }
    }

    private static bool IsBaseEntity(Type type)
    {
        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Domain.Entities.BaseEntity<>))
                return true;
            type = type.BaseType!;
        }
        return false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanFlowDbContext).Assembly);
    }
}
