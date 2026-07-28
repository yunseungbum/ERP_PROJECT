using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Members;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<User> Users => Set<User>();
    public DbSet<MatchSchedule> MatchSchedules => Set<MatchSchedule>();
    public DbSet<MatchParticipant> MatchParticipants => Set<MatchParticipant>();
    public DbSet<QuarterFormation> QuarterFormations => Set<QuarterFormation>();
    public DbSet<QuarterLineupPlayer> QuarterLineupPlayers =>
        Set<QuarterLineupPlayer>();
    public DbSet<MatchAttendance> MatchAttendances => Set<MatchAttendance>();
    public DbSet<InventoryPurchase> InventoryPurchases =>
        Set<InventoryPurchase>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<MemberDue> MemberDues => Set<MemberDue>();
    public DbSet<MemberDueNote> MemberDueNotes => Set<MemberDueNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var member = modelBuilder.Entity<Member>();

        member.ToTable("members");
        member.HasKey(x => x.MemberId);

        member.Property(x => x.MemberId)
            .HasColumnName("member_id")
            .ValueGeneratedOnAdd();

        member.Property(x => x.MemberName)
            .HasColumnName("member_name")
            .HasMaxLength(50)
            .IsRequired();

        member.Property(x => x.PrimaryPosition)
            .HasColumnName("primary_position")
            .HasMaxLength(30)
            .IsRequired();

        member.Property(x => x.SecondaryPosition)
            .HasColumnName("secondary_position")
            .HasMaxLength(30);

        member.Property(x => x.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(20)
            .IsRequired();

        member.Property(x => x.BirthYear)
            .HasColumnName("birth_year")
            .IsRequired();

        member.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000)
            .IsRequired();

        member.Property(x => x.MemberStatus)
            .HasColumnName("member_status")
            .HasMaxLength(20)
            .HasDefaultValue(MemberStatusCodes.Active)
            .IsRequired();

        member.Property(x => x.HasUniform)
            .HasColumnName("has_uniform")
            .HasDefaultValue(false)
            .IsRequired();

        member.Property(x => x.UniformNumber)
            .HasColumnName("uniform_number");

        member.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        member.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        member.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        var user = modelBuilder.Entity<User>();

        user.ToTable("users");
        user.HasKey(x => x.UserId);

        user.Property(x => x.UserId)
            .HasColumnName("user_id")
            .ValueGeneratedOnAdd();

        user.Property(x => x.MemberId)
            .HasColumnName("member_id");

        user.Property(x => x.LoginId)
            .HasColumnName("login_id")
            .HasMaxLength(50)
            .IsRequired();

        user.HasIndex(x => x.LoginId)
            .IsUnique();

        user.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(500)
            .IsRequired();

        user.Property(x => x.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(50)
            .IsRequired();

        user.Property(x => x.RoleCode)
            .HasColumnName("role_code")
            .HasMaxLength(30)
            .IsRequired();

        user.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        user.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        user.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        user.HasOne(x => x.Member)
            .WithOne()
            .HasForeignKey<User>(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        var matchSchedule = modelBuilder.Entity<MatchSchedule>();

        matchSchedule.ToTable("match_schedules");
        matchSchedule.HasKey(x => x.ScheduleId);

        matchSchedule.Property(x => x.ScheduleId)
            .HasColumnName("schedule_id")
            .ValueGeneratedOnAdd();

        matchSchedule.Property(x => x.VenueName)
            .HasColumnName("venue_name")
            .HasMaxLength(100)
            .IsRequired();

        matchSchedule.Property(x => x.OpponentName)
            .HasColumnName("opponent_name")
            .HasMaxLength(100)
            .IsRequired();

        matchSchedule.Property(x => x.StartsAt)
            .HasColumnName("starts_at")
            .IsRequired();

        matchSchedule.Property(x => x.MatchFee)
            .HasColumnName("match_fee")
            .HasPrecision(12, 0)
            .IsRequired();

        matchSchedule.Property(x => x.IsMatchFeePaid)
            .HasColumnName("is_match_fee_paid")
            .HasDefaultValue(false)
            .IsRequired();

        matchSchedule.Property(x => x.PayerName)
            .HasColumnName("payer_name")
            .HasMaxLength(50)
            .IsRequired();

        matchSchedule.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000)
            .IsRequired();

        matchSchedule.Property(x => x.IsCompleted)
            .HasColumnName("is_completed")
            .HasDefaultValue(false)
            .IsRequired();

        matchSchedule.Property(x => x.OpponentContact)
            .HasColumnName("opponent_contact")
            .HasMaxLength(30);

        matchSchedule.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        matchSchedule.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        matchSchedule.HasData(new MatchSchedule
        {
            ScheduleId = 1,
            VenueName = "신트리 공원",
            OpponentName = "신풍 FC",
            StartsAt = new DateTime(2026, 8, 20, 20, 0, 0),
            MatchFee = 0,
            IsMatchFeePaid = false,
            PayerName = "윤승범",
            Notes = string.Empty,
            IsCompleted = false,
            OpponentContact = null,
            CreatedAt = new DateTime(2026, 7, 24, 0, 0, 0),
            UpdatedAt = new DateTime(2026, 7, 24, 0, 0, 0),
        });

        var matchParticipant = modelBuilder.Entity<MatchParticipant>();

        matchParticipant.ToTable("match_participants");
        matchParticipant.HasKey(x => x.ParticipantId);

        matchParticipant.Property(x => x.ParticipantId)
            .HasColumnName("participant_id")
            .ValueGeneratedOnAdd();

        matchParticipant.Property(x => x.ScheduleId)
            .HasColumnName("schedule_id");

        matchParticipant.Property(x => x.MemberId)
            .HasColumnName("member_id");

        matchParticipant.Property(x => x.GuestName)
            .HasColumnName("guest_name")
            .HasMaxLength(50);

        matchParticipant.Property(x => x.IsGuest)
            .HasColumnName("is_guest")
            .IsRequired();

        matchParticipant.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        matchParticipant.HasIndex(x => new { x.ScheduleId, x.MemberId })
            .IsUnique();

        matchParticipant.HasIndex(x => new { x.ScheduleId, x.GuestName })
            .IsUnique();

        matchParticipant.HasOne(x => x.Schedule)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        matchParticipant.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        var quarterFormation = modelBuilder.Entity<QuarterFormation>();

        quarterFormation.ToTable("quarter_formations");
        quarterFormation.HasKey(x => x.QuarterFormationId);

        quarterFormation.Property(x => x.QuarterFormationId)
            .HasColumnName("quarter_formation_id")
            .ValueGeneratedOnAdd();

        quarterFormation.Property(x => x.ScheduleId)
            .HasColumnName("schedule_id");

        quarterFormation.Property(x => x.QuarterNumber)
            .HasColumnName("quarter_number");

        quarterFormation.Property(x => x.FormationCode)
            .HasColumnName("formation_code")
            .HasMaxLength(20)
            .IsRequired();

        quarterFormation.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        quarterFormation.HasIndex(x => new
            {
                x.ScheduleId,
                x.QuarterNumber,
            })
            .IsUnique();

        quarterFormation.HasOne(x => x.Schedule)
            .WithMany(x => x.QuarterFormations)
            .HasForeignKey(x => x.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        var lineupPlayer = modelBuilder.Entity<QuarterLineupPlayer>();

        lineupPlayer.ToTable("quarter_lineup_players");
        lineupPlayer.HasKey(x => x.LineupPlayerId);

        lineupPlayer.Property(x => x.LineupPlayerId)
            .HasColumnName("lineup_player_id")
            .ValueGeneratedOnAdd();

        lineupPlayer.Property(x => x.QuarterFormationId)
            .HasColumnName("quarter_formation_id");

        lineupPlayer.Property(x => x.ParticipantId)
            .HasColumnName("participant_id");

        lineupPlayer.Property(x => x.SlotCode)
            .HasColumnName("slot_code")
            .HasMaxLength(50)
            .IsRequired();

        lineupPlayer.Property(x => x.PositionOrder)
            .HasColumnName("position_order");

        lineupPlayer.HasIndex(x => new
            {
                x.QuarterFormationId,
                x.SlotCode,
            })
            .IsUnique();

        lineupPlayer.HasIndex(x => new
            {
                x.QuarterFormationId,
                x.ParticipantId,
            })
            .IsUnique();

        lineupPlayer.HasOne(x => x.QuarterFormation)
            .WithMany(x => x.LineupPlayers)
            .HasForeignKey(x => x.QuarterFormationId)
            .OnDelete(DeleteBehavior.Cascade);

        lineupPlayer.HasOne(x => x.Participant)
            .WithMany(x => x.LineupPlayers)
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        var matchAttendance = modelBuilder.Entity<MatchAttendance>();

        matchAttendance.ToTable("match_attendances");
        matchAttendance.HasKey(x => x.AttendanceId);

        matchAttendance.Property(x => x.AttendanceId)
            .HasColumnName("attendance_id")
            .ValueGeneratedOnAdd();

        matchAttendance.Property(x => x.ScheduleId)
            .HasColumnName("schedule_id");

        matchAttendance.Property(x => x.MemberId)
            .HasColumnName("member_id");

        matchAttendance.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(1)
            .IsRequired();

        matchAttendance.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        matchAttendance.HasIndex(x => new
            {
                x.ScheduleId,
                x.MemberId,
            })
            .IsUnique();

        matchAttendance.HasOne(x => x.Schedule)
            .WithMany()
            .HasForeignKey(x => x.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        matchAttendance.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        var inventoryPurchase = modelBuilder.Entity<InventoryPurchase>();

        inventoryPurchase.ToTable("inventory_purchases");
        inventoryPurchase.HasKey(x => x.PurchaseId);
        inventoryPurchase.Property(x => x.PurchaseId)
            .HasColumnName("purchase_id")
            .ValueGeneratedOnAdd();
        inventoryPurchase.Property(x => x.ItemName)
            .HasColumnName("item_name")
            .HasMaxLength(100)
            .IsRequired();
        inventoryPurchase.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();
        inventoryPurchase.Property(x => x.Amount)
            .HasColumnName("amount")
            .HasPrecision(12, 0)
            .IsRequired();
        inventoryPurchase.Property(x => x.IsPurchased)
            .HasColumnName("is_purchased")
            .HasDefaultValue(false)
            .IsRequired();
        inventoryPurchase.Property(x => x.PurchasedAt)
            .HasColumnName("purchased_at");
        inventoryPurchase.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        inventoryPurchase.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        var inventoryItem = modelBuilder.Entity<InventoryItem>();

        inventoryItem.ToTable("inventory_items");
        inventoryItem.HasKey(x => x.InventoryItemId);
        inventoryItem.Property(x => x.InventoryItemId)
            .HasColumnName("inventory_item_id")
            .ValueGeneratedOnAdd();
        inventoryItem.Property(x => x.ItemName)
            .HasColumnName("item_name")
            .HasMaxLength(100)
            .IsRequired();
        inventoryItem.HasIndex(x => x.ItemName)
            .IsUnique();
        inventoryItem.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();
        inventoryItem.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        inventoryItem.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        var expense = modelBuilder.Entity<Expense>();

        expense.ToTable("expenses");
        expense.HasKey(x => x.ExpenseId);
        expense.Property(x => x.ExpenseId)
            .HasColumnName("expense_id")
            .ValueGeneratedOnAdd();
        expense.Property(x => x.ScheduleId)
            .HasColumnName("schedule_id");
        expense.HasIndex(x => x.ScheduleId)
            .IsUnique();
        expense.Property(x => x.ExpenseItem)
            .HasColumnName("expense_item")
            .HasMaxLength(100)
            .IsRequired();
        expense.Property(x => x.Amount)
            .HasColumnName("amount")
            .HasPrecision(12, 0)
            .IsRequired();
        expense.Property(x => x.PaymentDate)
            .HasColumnName("payment_date")
            .IsRequired();
        expense.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000)
            .IsRequired();
        expense.Property(x => x.PayerName)
            .HasColumnName("payer_name")
            .HasMaxLength(50)
            .IsRequired();
        expense.Property(x => x.IsSettled)
            .HasColumnName("is_settled")
            .HasDefaultValue(false)
            .IsRequired();
        expense.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        expense.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        expense.HasOne(x => x.Schedule)
            .WithOne(x => x.Expense)
            .HasForeignKey<Expense>(x => x.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        var announcement = modelBuilder.Entity<Announcement>();

        announcement.ToTable("announcements");
        announcement.HasKey(x => x.AnnouncementId);
        announcement.Property(x => x.AnnouncementId)
            .HasColumnName("announcement_id")
            .ValueGeneratedOnAdd();
        announcement.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(100)
            .IsRequired();
        announcement.Property(x => x.Content)
            .HasColumnName("content")
            .HasMaxLength(1000)
            .IsRequired();
        announcement.Property(x => x.AuthorName)
            .HasColumnName("author_name")
            .HasMaxLength(50)
            .IsRequired();
        announcement.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        announcement.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        var memberDue = modelBuilder.Entity<MemberDue>();

        memberDue.ToTable("member_dues");
        memberDue.HasKey(x => x.MemberDueId);
        memberDue.Property(x => x.MemberDueId)
            .HasColumnName("member_due_id")
            .ValueGeneratedOnAdd();
        memberDue.Property(x => x.MemberId)
            .HasColumnName("member_id");
        memberDue.Property(x => x.DueYear)
            .HasColumnName("due_year");
        memberDue.Property(x => x.DueMonth)
            .HasColumnName("due_month");
        memberDue.Property(x => x.Amount)
            .HasColumnName("amount")
            .HasPrecision(12, 0)
            .IsRequired();
        memberDue.Property(x => x.PaymentStatus)
            .HasColumnName("payment_status")
            .HasMaxLength(20)
            .IsRequired();
        memberDue.Property(x => x.PaidAt)
            .HasColumnName("paid_at");
        memberDue.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        memberDue.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        memberDue.HasIndex(x => new
            {
                x.MemberId,
                x.DueYear,
                x.DueMonth,
            })
            .IsUnique();
        memberDue.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        var memberDueNote = modelBuilder.Entity<MemberDueNote>();

        memberDueNote.ToTable("member_due_notes");
        memberDueNote.HasKey(x => x.MemberDueNoteId);
        memberDueNote.Property(x => x.MemberDueNoteId)
            .HasColumnName("member_due_note_id")
            .ValueGeneratedOnAdd();
        memberDueNote.Property(x => x.MemberId)
            .HasColumnName("member_id");
        memberDueNote.Property(x => x.DueYear)
            .HasColumnName("due_year");
        memberDueNote.Property(x => x.Content)
            .HasColumnName("content")
            .HasMaxLength(1000)
            .IsRequired();
        memberDueNote.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        memberDueNote.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        memberDueNote.HasIndex(x => new
            {
                x.MemberId,
                x.DueYear,
            })
            .IsUnique();
        memberDueNote.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
