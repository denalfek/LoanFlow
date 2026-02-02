using LoanFlow.Domain.Entities;
using LoanFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly LoanFlowDbContext _dbContext;

    public LoansController(LoanFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
    {
        var loans = await _dbContext.Loans
            .Where(l => !l.Deleted)
            .ToListAsync();

        return Ok(loans);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Loan>> GetLoan(Guid id)
    {
        var loan = await _dbContext.Loans
            .FirstOrDefaultAsync(l => l.Id == id && !l.Deleted);

        if (loan is null)
            return NotFound();

        return Ok(loan);
    }
}
