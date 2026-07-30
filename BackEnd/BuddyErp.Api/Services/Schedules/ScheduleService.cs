using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Schedules;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Services.Schedules;

public sealed class ScheduleService(AppDbContext dbContext) : IScheduleService
{
    private static readonly HashSet<string> AllowedPayerNames =
    [
        "윤승범",
        "김찬욱",
        "윤진혁",
        "홍준수",
        "김주빈",
    ];

    public async Task<IReadOnlyList<ScheduleResponse>> GetSchedulesAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.MatchSchedules
            .AsNoTracking()
            .OrderByDescending(schedule => schedule.StartsAt)
            .Select(schedule => new ScheduleResponse(
                schedule.ScheduleId,
                schedule.VenueName,
                schedule.OpponentName,
                schedule.StartsAt,
                schedule.MatchFee,
                schedule.IsMatchFeePaid,
                schedule.PayerName,
                schedule.Notes,
                schedule.IsCompleted,
                schedule.OpponentContact))
            .ToListAsync(cancellationToken);
    }

    public async Task<ScheduleResponse> CreateScheduleAsync(
        ScheduleSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var payerName = ValidatePayerName(request.PayerName);
        var now = DateTime.UtcNow;
        var schedule = new MatchSchedule
        {
            StartsAt = request.StartsAt,
            OpponentName = request.OpponentName.Trim(),
            VenueName = request.VenueName.Trim(),
            MatchFee = request.MatchFee,
            IsMatchFeePaid = request.IsMatchFeePaid,
            PayerName = payerName,
            Notes = request.Notes?.Trim() ?? string.Empty,
            IsCompleted = request.IsCompleted,
            OpponentContact = NullIfWhiteSpace(request.OpponentContact),
            CreatedAt = now,
            UpdatedAt = now,
        };
        var expense = CreateVenueExpense(schedule, now);

        dbContext.MatchSchedules.Add(schedule);
        dbContext.Expenses.Add(expense);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(schedule);
    }

    public async Task<ScheduleResponse?> UpdateScheduleAsync(
        long scheduleId,
        ScheduleSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var payerName = ValidatePayerName(request.PayerName);
        var schedule = await dbContext.MatchSchedules
            .Include(item => item.Expense)
            .SingleOrDefaultAsync(
            schedule => schedule.ScheduleId == scheduleId,
            cancellationToken);

        if (schedule is null)
        {
            return null;
        }

        schedule.StartsAt = request.StartsAt;
        schedule.OpponentName = request.OpponentName.Trim();
        schedule.VenueName = request.VenueName.Trim();
        schedule.MatchFee = request.MatchFee;
        schedule.IsMatchFeePaid = request.IsMatchFeePaid;
        schedule.PayerName = payerName;
        schedule.Notes = request.Notes?.Trim() ?? string.Empty;
        schedule.IsCompleted = request.IsCompleted;
        schedule.OpponentContact = NullIfWhiteSpace(request.OpponentContact);
        schedule.UpdatedAt = DateTime.UtcNow;

        if (schedule.Expense is null)
        {
            dbContext.Expenses.Add(
                CreateVenueExpense(schedule, schedule.UpdatedAt));
        }
        else
        {
            SyncVenueExpense(schedule.Expense, schedule);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(schedule);
    }

    public async Task<bool> DeleteScheduleAsync(
        long scheduleId,
        CancellationToken cancellationToken = default)
    {
        var schedule = await dbContext.MatchSchedules.SingleOrDefaultAsync(
            schedule => schedule.ScheduleId == scheduleId,
            cancellationToken);

        if (schedule is null)
        {
            return false;
        }

        dbContext.MatchSchedules.Remove(schedule);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ScheduleResponse ToResponse(MatchSchedule schedule)
    {
        return new ScheduleResponse(
            schedule.ScheduleId,
            schedule.VenueName,
            schedule.OpponentName,
            schedule.StartsAt,
            schedule.MatchFee,
            schedule.IsMatchFeePaid,
            schedule.PayerName,
            schedule.Notes,
            schedule.IsCompleted,
            schedule.OpponentContact);
    }

    private static Expense CreateVenueExpense(
        MatchSchedule schedule,
        DateTime now)
    {
        return new Expense
        {
            Schedule = schedule,
            ExpenseItem = "구장비",
            Amount = schedule.MatchFee,
            PaymentDate = schedule.StartsAt,
            Notes = BuildVenueExpenseNotes(schedule),
            PayerName = schedule.PayerName,
            IsSettled = false,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    private static void SyncVenueExpense(
        Expense expense,
        MatchSchedule schedule)
    {
        expense.ExpenseItem = "구장비";
        expense.Amount = schedule.MatchFee;
        expense.PaymentDate = schedule.StartsAt;
        expense.Notes = BuildVenueExpenseNotes(schedule);
        expense.PayerName = schedule.PayerName;
        expense.UpdatedAt = schedule.UpdatedAt;
    }

    private static string BuildVenueExpenseNotes(MatchSchedule schedule)
    {
        return $"{schedule.StartsAt:yyyy.MM.dd} {schedule.VenueName} " +
            $"{schedule.StartsAt:HH:mm}";
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

    private static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
