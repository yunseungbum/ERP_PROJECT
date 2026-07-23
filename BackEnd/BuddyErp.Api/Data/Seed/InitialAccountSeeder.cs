using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Auth;
using BuddyErp.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BuddyErp.Api.Data.Seed;

public sealed class InitialAccountSeeder(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    IOptions<InitialAccountPasswordsOptions> passwordOptions)
{
    private static readonly InitialAccountDefinition[] AccountDefinitions =
    [
        new("president", "회장", UserRoleCodes.President),
        new("director", "감독", UserRoleCodes.Director),
        new("coach", "코치", UserRoleCodes.Coach),
        new("treasurer", "총무", UserRoleCodes.Treasurer),
        new("inventory", "물품담당", UserRoleCodes.InventoryManager),
        new("member", "일반", UserRoleCodes.Member),
        new("guest", "Guest", UserRoleCodes.Member),
    ];

    public async Task<int> SeedAsync(CancellationToken cancellationToken = default)
    {
        var existingLoginIds = await dbContext.Users
            .AsNoTracking()
            .Select(user => user.LoginId)
            .ToListAsync(cancellationToken);

        var existingLoginIdSet = existingLoginIds.ToHashSet(
            StringComparer.OrdinalIgnoreCase);
        var missingDefinitions = AccountDefinitions
            .Where(definition =>
                !existingLoginIdSet.Contains(definition.LoginId))
            .ToArray();

        if (missingDefinitions.Length == 0)
        {
            return 0;
        }

        var passwords = GetPasswords();
        ValidatePasswords(passwords, missingDefinitions);
        var now = DateTime.UtcNow;

        foreach (var definition in missingDefinitions)
        {
            var user = new User
            {
                LoginId = definition.LoginId,
                DisplayName = definition.DisplayName,
                RoleCode = definition.RoleCode,
                PasswordHash = string.Empty,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
            };

            user.PasswordHash = passwordHasher.HashPassword(
                user,
                passwords[definition.LoginId]);

            dbContext.Users.Add(user);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return missingDefinitions.Length;
    }

    private Dictionary<string, string> GetPasswords()
    {
        var options = passwordOptions.Value;

        return new Dictionary<string, string>
        {
            ["president"] = options.President,
            ["director"] = options.Director,
            ["coach"] = options.Coach,
            ["treasurer"] = options.Treasurer,
            ["inventory"] = options.InventoryManager,
            ["member"] = options.Member,
            ["guest"] = options.Guest,
        };
    }

    private static void ValidatePasswords(
        IReadOnlyDictionary<string, string> passwords,
        IReadOnlyCollection<InitialAccountDefinition> missingDefinitions)
    {
        var missingRoles = missingDefinitions
            .Where(definition =>
                string.IsNullOrWhiteSpace(passwords[definition.LoginId]))
            .Select(definition => definition.LoginId)
            .ToArray();

        if (missingRoles.Length > 0)
        {
            throw new InvalidOperationException(
                "초기 계정 비밀번호 User Secrets 설정이 필요합니다: " +
                string.Join(", ", missingRoles));
        }
    }

    private sealed record InitialAccountDefinition(
        string LoginId,
        string DisplayName,
        string RoleCode);
}
