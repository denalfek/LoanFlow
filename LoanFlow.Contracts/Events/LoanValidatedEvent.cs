namespace LoanFlow.Contracts.Events;

public record LoanValidatedEvent(
    Guid LoanApplicationId,
    Guid ApplicantId,
    decimal Amount,
    bool IsValid,
    string[] ValidationErrors,
    DateTime ValidatedAt);
