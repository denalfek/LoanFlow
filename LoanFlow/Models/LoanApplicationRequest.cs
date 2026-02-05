namespace LoanFlow.Api.Models;

public record LoanApplicationRequest(
    Guid ApplicantId,
    decimal Amount,
    string Purpose);
