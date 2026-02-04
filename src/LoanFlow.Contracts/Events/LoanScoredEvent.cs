namespace LoanFlow.Contracts.Events;

public record LoanScoredEvent(
    Guid LoanApplicationId,
    Guid ApplicantId,
    short CreditScore,
    string RiskLevel,
    DateTime ScoredAt);
