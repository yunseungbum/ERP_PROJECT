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
    }
}
