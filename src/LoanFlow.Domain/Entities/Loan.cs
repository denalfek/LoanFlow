namespace LoanFlow.Domain.Entities;

public class Loan : BaseEntity<Guid>
{
    public string BorrowerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public LoanStatus Status { get; set; }
}

public enum LoanStatus
{
    Pending,
    Approved,
    Rejected,
    Active,
    Completed
}
