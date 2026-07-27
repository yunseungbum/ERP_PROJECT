using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Expenses;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Services.Expenses;

public sealed class ExpenseService(AppDbContext dbContext) : IExpenseService
{
    private static readonly string[] UnsettledPayerNames =
    [
        "윤승범",
        "김찬욱",
        "윤진혁",
        "홍준수",
        "김주빈",
    ];

    private static readonly HashSet<string> AllowedPayerNames =
    [
        .. UnsettledPayerNames,
        "회비",
    ];

    public async Task<ExpenseSummaryResponse> GetExpensesAsync(
        CancellationToken cancellationToken = default)
    {
        var expenses = await dbContext.Expenses
            .AsNoTracking()
            .OrderByDescending(expense => expense.PaymentDate)
            .ThenByDescending(expense => expense.ExpenseId)
            .Select(expense => new ExpenseResponse(
                expense.ExpenseId,
                expense.ScheduleId,
                expense.ExpenseItem,
                expense.Amount,
                expense.PaymentDate,
                expense.Notes,
                expense.PayerName,
                expense.IsSettled))
            .ToListAsync(cancellationToken);

        var unsettledAmounts = UnsettledPayerNames.ToDictionary(
            payerName => payerName,
            payerName => expenses
                .Where(expense =>
                    expense.PayerName == payerName &&
                    !expense.IsSettled)
                .Sum(expense => expense.Amount));

        return new ExpenseSummaryResponse(
            expenses.Sum(expense => expense.Amount),
            unsettledAmounts,
            expenses);
    }

    public async Task<ExpenseResponse?> UpdateSettlementAsync(
        long expenseId,
        ExpenseSettlementRequest request,
        CancellationToken cancellationToken = default)
    {
        var expense = await dbContext.Expenses
            .Include(item => item.Schedule)
            .SingleOrDefaultAsync(
                item => item.ExpenseId == expenseId,
                cancellationToken);

        if (expense is null)
        {
            return null;
        }

        expense.IsSettled = request.IsSettled;
        expense.UpdatedAt = DateTime.UtcNow;

        if (expense.Schedule is not null)
        {
            expense.Schedule.IsMatchFeePaid = request.IsSettled;
            expense.Schedule.UpdatedAt = expense.UpdatedAt;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ExpenseResponse(
            expense.ExpenseId,
            expense.ScheduleId,
            expense.ExpenseItem,
            expense.Amount,
            expense.PaymentDate,
            expense.Notes,
            expense.PayerName,
            expense.IsSettled);
    }

    public async Task<ExpenseResponse> CreateExpenseAsync(
        ExpenseSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var expense = new Expense
        {
            ScheduleId = null,
            ExpenseItem = ValidateExpenseItem(request.ExpenseItem),
            Amount = request.Amount,
            PaymentDate = request.PaymentDate,
            Notes = request.Notes?.Trim() ?? string.Empty,
            PayerName = ValidatePayerName(request.PayerName),
            IsSettled = request.IsSettled,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.Expenses.Add(expense);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(expense);
    }

    public async Task<ExpenseResponse?> UpdateExpenseAsync(
        long expenseId,
        ExpenseSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var expense = await dbContext.Expenses.SingleOrDefaultAsync(
            item => item.ExpenseId == expenseId,
            cancellationToken);

        if (expense is null)
        {
            return null;
        }

        EnsureManualExpense(expense);
        expense.ExpenseItem = ValidateExpenseItem(request.ExpenseItem);
        expense.Amount = request.Amount;
        expense.PaymentDate = request.PaymentDate;
        expense.Notes = request.Notes?.Trim() ?? string.Empty;
        expense.PayerName = ValidatePayerName(request.PayerName);
        expense.IsSettled = request.IsSettled;
        expense.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(expense);
    }

    public async Task<bool> DeleteExpenseAsync(
        long expenseId,
        CancellationToken cancellationToken = default)
    {
        var expense = await dbContext.Expenses.SingleOrDefaultAsync(
            item => item.ExpenseId == expenseId,
            cancellationToken);

        if (expense is null)
        {
            return false;
        }

        EnsureManualExpense(expense);
        dbContext.Expenses.Remove(expense);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ExpenseResponse ToResponse(Expense expense)
    {
        return new ExpenseResponse(
            expense.ExpenseId,
            expense.ScheduleId,
            expense.ExpenseItem,
            expense.Amount,
            expense.PaymentDate,
            expense.Notes,
            expense.PayerName,
            expense.IsSettled);
    }

    private static string ValidateExpenseItem(string expenseItem)
    {
        var trimmedExpenseItem = expenseItem.Trim();
        if (trimmedExpenseItem.Length == 0)
        {
            throw new ArgumentException("지출항목을 입력해 주세요.");
        }

        return trimmedExpenseItem;
    }

    private static string ValidatePayerName(string payerName)
    {
        var trimmedPayerName = payerName.Trim();
        if (!AllowedPayerNames.Contains(trimmedPayerName))
        {
            throw new ArgumentException("결제 인원을 올바르게 선택해 주세요.");
        }

        return trimmedPayerName;
    }

    private static void EnsureManualExpense(Expense expense)
    {
        if (expense.ScheduleId.HasValue)
        {
            throw new InvalidOperationException(
                "경기 일정에서 생성된 구장비는 경기 일정 화면에서 변경해 주세요.");
        }
    }
}
