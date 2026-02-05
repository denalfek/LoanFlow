namespace LoanFlow.Domain.Entities;

public class LoanDecision : BaseEntity<Guid>
{
    public required Guid LoanApplicationId { get; set; }

    public Int16 CreditScore { get; set; }

    public required string RiskLevel { get; set; }

    public string? Descision { get; set; }

    public  required string Reason { get; set; }

    public DateTime? Decidet { get; set; }

    public virtual required LoanApplication Application { get; set; }
}
