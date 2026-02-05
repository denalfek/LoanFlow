using LoanFlow.Domain.Entities;

namespace LoanFlow.Api.Models;

public record LoanApplicationResponse(
    Guid Id,
    Guid ApplicantId,
    decimal Amount,
    string Purpose,
    string Status,
    DateTime Submitted,
    DateTime Created);
