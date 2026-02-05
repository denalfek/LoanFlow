namespace LoanFlow.Api.Models;

public record ApplicantResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    DateOnly DateOfBirth,
    decimal MonthlyIncome,
    DateTime Created);
