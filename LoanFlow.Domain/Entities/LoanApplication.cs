namespace LoanFlow.Domain.Entities;

public class LoanApplication : BaseEntity<Guid>
{
    public Guid ApplicantId { get; set; }
    public decimal Amount { get; set; }
    public required string Purpose { get; set; }
    public LoanApplicationStatus Status { get; set; }

    public DateTime Submitted {  get; set; }

    public virtual required Applicant Applicant { get; set; }

    public virtual LoanDecision? LoanDecision { get; set; }
}

public enum LoanApplicationStatus
{
    Submitted,
    UnderReview,
    Approved,
    Rejected,
    Disbursed
}