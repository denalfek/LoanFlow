namespace LoanFlow.Domain.Entities;

public class Applicant : BaseEntity<Guid>
{
    public required string FirstName {  get; set; }

    public required string LastName { get; set; }

    public string? Email { get; set; }
    public DateOnly DateOfBirth { get; set; }

    public decimal MonthlyIncome { get; set; }

    public virtual ICollection<LoanApplication> LoanApplications { get; set; } = [];
}
