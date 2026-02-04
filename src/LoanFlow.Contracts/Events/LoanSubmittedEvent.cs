namespace LoanFlow.Contracts.Events;

public record LoanSubmittedEvent(
    Guid LoanApplicationId,
    Guid ApplicantId,
    decimal Amount,
    string Purpose,
    DateTime SubmittedAt);
