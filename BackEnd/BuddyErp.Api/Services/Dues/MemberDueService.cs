using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Dues;
using BuddyErp.Api.DTOs.Members;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Services.Dues;

public sealed class MemberDueService(
    AppDbContext dbContext) : IMemberDueService
{
    private const decimal MonthlyFee = 20_000m;
    private const decimal UniformFee = 50_000m;
    private const int DueDay = 20;
    private const string PaidStatus = "Paid";
    private const string UnpaidStatus = "Unpaid";
    private const string ExemptStatus = "Exempt";
    private const int FirstDuesYear = 2026;
    private const int FirstDuesMonth = 5;

    public async Task<DuesMatrixResponse> GetDuesMatrixAsync(
        int year,
        CancellationToken cancellationToken = default)
    {
        ValidateYear(year);
        var today = DateTime.Today;
        var members = await dbContext.Members
            .AsNoTracking()
            .Where(member => member.IsActive)
            .OrderBy(member => member.MemberId)
            .Select(member => new
            {
                member.MemberId,
                member.MemberName,
                member.MemberStatus,
                member.HasUniform,
            })
            .ToListAsync(cancellationToken);

        var existingDues = await dbContext.MemberDues
            .Where(due => due.DueYear == year)
            .ToListAsync(cancellationToken);
        var dueDetails = await dbContext.MemberDueNotes
            .AsNoTracking()
            .Where(note => note.DueYear == year)
            .ToDictionaryAsync(
                note => note.MemberId,
                note => note,
                cancellationToken);
        var summaryNote = await dbContext.DuesYearSummaries
            .AsNoTracking()
            .Where(summary => summary.DueYear == year)
            .Select(summary => summary.Notes)
            .SingleOrDefaultAsync(cancellationToken)
            ?? string.Empty;
        var totalExpenseAmount = await dbContext.Expenses
            .AsNoTracking()
            .Where(expense => expense.PaymentDate.Year == year)
            .SumAsync(
                expense => (decimal?)expense.Amount,
                cancellationToken)
            ?? 0;
        var dueLookup = existingDues.ToDictionary(
            due => (due.MemberId, due.DueMonth));
        var newDues = new List<MemberDue>();
        var now = DateTime.UtcNow;

        foreach (var member in members)
        {
            var isPaused =
                member.MemberStatus == MemberStatusCodes.Paused;

            for (var month = 1; month <= 12; month++)
            {
                var dueDate = new DateTime(year, month, DueDay);
                if (dueDate > today ||
                    dueLookup.ContainsKey((member.MemberId, month)))
                {
                    continue;
                }

                var due = new MemberDue
                {
                    MemberId = member.MemberId,
                    DueYear = year,
                    DueMonth = month,
                    Amount = MonthlyFee,
                    PaymentStatus = IsBeforeDuesStart(year, month) ||
                        isPaused
                        ? ExemptStatus
                        : UnpaidStatus,
                    PaidAt = null,
                    CreatedAt = now,
                    UpdatedAt = now,
                };
                dueLookup[(member.MemberId, month)] = due;
                newDues.Add(due);
            }
        }

        if (newDues.Count > 0)
        {
            dbContext.MemberDues.AddRange(newDues);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var memberResponses = members.Select(member =>
        {
            var isPaused =
                member.MemberStatus == MemberStatusCodes.Paused;
            var cells = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var dueDate = new DateTime(year, month, DueDay);
                    if (IsBeforeDuesStart(year, month))
                    {
                        return new DuesCellResponse(month, dueDate, "-");
                    }

                    if (isPaused)
                    {
                        if (dueLookup.TryGetValue(
                            (member.MemberId, month),
                            out var pausedDue))
                        {
                            return new DuesCellResponse(
                                month,
                                dueDate,
                                ToDisplayStatus(pausedDue.PaymentStatus));
                        }

                        return new DuesCellResponse(month, dueDate, "-");
                    }

                    if (dueLookup.TryGetValue(
                        (member.MemberId, month),
                        out var due))
                    {
                        return new DuesCellResponse(
                            month,
                            dueDate,
                            ToDisplayStatus(due.PaymentStatus));
                    }

                    return new DuesCellResponse(month, dueDate, "·");
                })
                .ToList();

            var paidTotal =
                cells.Count(cell => cell.Status == "O") * MonthlyFee +
                (member.HasUniform ? UniformFee : 0);
            var unpaidTotal = isPaused
                ? 0
                : cells.Count(cell => cell.Status == "X") * MonthlyFee;

            return new DuesMemberResponse(
                member.MemberId,
                member.MemberName,
                isPaused,
                member.HasUniform,
                dueDetails.GetValueOrDefault(member.MemberId)
                    ?.ExecutionAmount ?? 0,
                paidTotal,
                unpaidTotal,
                dueDetails.GetValueOrDefault(member.MemberId)
                    ?.Content ?? string.Empty,
                cells);
        }).ToList();

        var totalExecutionAmount = memberResponses.Sum(
            member => member.ExecutionAmount);
        var totalPaidAmount = memberResponses.Sum(
            member => member.PaidTotal);

        return new DuesMatrixResponse(
            year,
            MonthlyFee,
            totalExecutionAmount,
            totalPaidAmount,
            totalExpenseAmount,
            totalPaidAmount - totalExpenseAmount,
            memberResponses.Sum(member => member.UnpaidTotal),
            memberResponses.Count(member => member.UnpaidTotal > 0),
            summaryNote,
            memberResponses);
    }

    public async Task<DuesCellResponse?> UpdateDueAsync(
        long memberId,
        int year,
        int month,
        DuesUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateYear(year);
        if (month is < 1 or > 12)
        {
            throw new ArgumentException("회비 월을 확인해 주세요.");
        }
        if (IsBeforeDuesStart(year, month))
        {
            throw new ArgumentException(
                "2026년 1월부터 4월까지는 회비 대상이 아닙니다.");
        }

        var status = request.Status.Trim().ToUpperInvariant();
        if (status is not ("O" or "X" or "-"))
        {
            throw new ArgumentException(
                "회비 상태는 O, X 또는 -만 가능합니다.");
        }

        var member = await dbContext.Members.SingleOrDefaultAsync(
            item => item.MemberId == memberId && item.IsActive,
            cancellationToken);
        if (member is null)
        {
            return null;
        }
        var dueDate = new DateTime(year, month, DueDay);
        var due = await dbContext.MemberDues.SingleOrDefaultAsync(
            item =>
                item.MemberId == memberId &&
                item.DueYear == year &&
                item.DueMonth == month,
            cancellationToken);
        var now = DateTime.UtcNow;

        if (due is null)
        {
            due = new MemberDue
            {
                MemberId = memberId,
                DueYear = year,
                DueMonth = month,
                Amount = MonthlyFee,
                PaymentStatus = ToPaymentStatus(status),
                PaidAt = status == "O" ? now : null,
                CreatedAt = now,
                UpdatedAt = now,
            };
            dbContext.MemberDues.Add(due);
        }
        else
        {
            due.PaymentStatus = ToPaymentStatus(status);
            due.PaidAt = status == "O" ? now : null;
            due.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new DuesCellResponse(month, dueDate, status);
    }

    public async Task<DuesNoteResponse?> UpdateDueNoteAsync(
        long memberId,
        int year,
        DuesNoteUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateYear(year);
        var memberExists = await dbContext.Members.AnyAsync(
            member => member.MemberId == memberId && member.IsActive,
            cancellationToken);
        if (!memberExists)
        {
            return null;
        }

        var content = request.Content?.Trim() ?? string.Empty;
        var note = await dbContext.MemberDueNotes.SingleOrDefaultAsync(
            item => item.MemberId == memberId && item.DueYear == year,
            cancellationToken);
        var now = DateTime.UtcNow;

        if (note is null)
        {
            note = new MemberDueNote
            {
                MemberId = memberId,
                DueYear = year,
                Content = content,
                CreatedAt = now,
                UpdatedAt = now,
            };
            dbContext.MemberDueNotes.Add(note);
        }
        else
        {
            note.Content = content;
            note.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new DuesNoteResponse(note.Content);
    }

    public async Task<DuesExecutionResponse?> UpdateExecutionAmountAsync(
        long memberId,
        int year,
        DuesExecutionUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateYear(year);
        var memberExists = await dbContext.Members.AnyAsync(
            member => member.MemberId == memberId && member.IsActive,
            cancellationToken);
        if (!memberExists)
        {
            return null;
        }

        var detail = await dbContext.MemberDueNotes.SingleOrDefaultAsync(
            item => item.MemberId == memberId && item.DueYear == year,
            cancellationToken);
        var now = DateTime.UtcNow;

        if (detail is null)
        {
            detail = new MemberDueNote
            {
                MemberId = memberId,
                DueYear = year,
                ExecutionAmount = request.Amount,
                Content = string.Empty,
                CreatedAt = now,
                UpdatedAt = now,
            };
            dbContext.MemberDueNotes.Add(detail);
        }
        else
        {
            detail.ExecutionAmount = request.Amount;
            detail.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new DuesExecutionResponse(detail.ExecutionAmount);
    }

    public async Task<DuesSummaryNoteResponse> UpdateSummaryNoteAsync(
        int year,
        DuesSummaryNoteUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateYear(year);
        var content = request.Content?.Trim() ?? string.Empty;
        var summary = await dbContext.DuesYearSummaries
            .SingleOrDefaultAsync(
                item => item.DueYear == year,
                cancellationToken);
        var now = DateTime.UtcNow;

        if (summary is null)
        {
            summary = new DuesYearSummary
            {
                DueYear = year,
                Notes = content,
                CreatedAt = now,
                UpdatedAt = now,
            };
            dbContext.DuesYearSummaries.Add(summary);
        }
        else
        {
            summary.Notes = content;
            summary.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new DuesSummaryNoteResponse(summary.Notes);
    }

    private static string ToDisplayStatus(string status)
    {
        return status switch
        {
            PaidStatus => "O",
            UnpaidStatus => "X",
            _ => "-",
        };
    }

    private static string ToPaymentStatus(string status)
    {
        return status switch
        {
            "O" => PaidStatus,
            "X" => UnpaidStatus,
            _ => ExemptStatus,
        };
    }

    private static void ValidateYear(int year)
    {
        if (year is < 2020 or > 2100)
        {
            throw new ArgumentException("회비 연도를 확인해 주세요.");
        }
    }

    private static bool IsBeforeDuesStart(int year, int month)
    {
        return year == FirstDuesYear && month < FirstDuesMonth;
    }
}
