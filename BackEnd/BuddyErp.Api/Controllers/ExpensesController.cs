using BuddyErp.Api.DTOs.Expenses;
using BuddyErp.Api.Services.Expenses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuddyErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/expenses")]
public sealed class ExpensesController(
    IExpenseService expenseService) : ControllerBase
{
    private const string ExpenseWriters = "President,Treasurer";

    [HttpGet]
    public async Task<ActionResult<ExpenseSummaryResponse>> GetExpenses(
        CancellationToken cancellationToken)
    {
        return Ok(await expenseService.GetExpensesAsync(cancellationToken));
    }

    [Authorize(Roles = ExpenseWriters)]
    [HttpPatch("{expenseId:long}/settlement")]
    public async Task<ActionResult<ExpenseResponse>> UpdateSettlement(
        long expenseId,
        ExpenseSettlementRequest request,
        CancellationToken cancellationToken)
    {
        var expense = await expenseService.UpdateSettlementAsync(
            expenseId,
            request,
            cancellationToken);
        return expense is null ? NotFound() : Ok(expense);
    }

    [Authorize(Roles = ExpenseWriters)]
    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> CreateExpense(
        ExpenseSaveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var expense = await expenseService.CreateExpenseAsync(
                request,
                cancellationToken);
            return Ok(expense);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
    }

    [Authorize(Roles = ExpenseWriters)]
    [HttpPut("{expenseId:long}")]
    public async Task<ActionResult<ExpenseResponse>> UpdateExpense(
        long expenseId,
        ExpenseSaveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var expense = await expenseService.UpdateExpenseAsync(
                expenseId,
                request,
                cancellationToken);
            return expense is null ? NotFound() : Ok(expense);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Detail = exception.Message });
        }
    }

    [Authorize(Roles = ExpenseWriters)]
    [HttpDelete("{expenseId:long}")]
    public async Task<IActionResult> DeleteExpense(
        long expenseId,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await expenseService.DeleteExpenseAsync(
                expenseId,
                cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Detail = exception.Message });
        }
    }
}
