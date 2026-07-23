namespace BuddyErp.Api.DTOs.Auth;

public static class UserRoleCodes
{
    public const string President = "President";
    public const string Director = "Director";
    public const string Coach = "Coach";
    public const string Treasurer = "Treasurer";
    public const string InventoryManager = "InventoryManager";
    public const string Member = "Member";

    private static readonly HashSet<string> ValidCodes =
    [
        President,
        Director,
        Coach,
        Treasurer,
        InventoryManager,
        Member,
    ];

    public static bool IsValid(string roleCode)
    {
        return ValidCodes.Contains(roleCode);
    }
}
