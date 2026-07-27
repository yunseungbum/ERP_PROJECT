using BuddyErp.Api.DTOs.Expenses;

namespace BuddyErp.Api.Services.Expenses;

public interface IExpenseService
{
    Task<ExpenseSummaryResponse> GetExpensesAsync(
        CancellationToken cancellationToken = default);

    Task<ExpenseResponse?> UpdateSettlementAsync(
        long expenseId,
        ExpenseSettlementRequest request,
        CancellationToken cancellationToken = default);

    Task<ExpenseResponse> CreateExpenseAsync(
        ExpenseSaveRequest request,
        CancellationToken cancellationToken = default);

    Task<ExpenseResponse?> UpdateExpenseAsync(
        long expenseId,
        ExpenseSaveRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteExpenseAsync(
        long expenseId,
        CancellationToken cancellationToken = default);
}
