using BuddyErp.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<User> Users => Set<User>();

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
    }
}
