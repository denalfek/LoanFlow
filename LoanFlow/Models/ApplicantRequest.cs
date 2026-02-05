namespace LoanFlow.Api.Models;

public record ApplicantRequest(
    string FirstName,
    string LastName,
    string? Email,
    DateOnly DateOfBirth,
    decimal MonthlyIncome);
