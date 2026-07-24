namespace BuddyErp.Api.DTOs.Members;

public static class MemberStatusCodes
{
    public const string Active = "Active";
    public const string Paused = "Paused";

    public const string ValidationPattern =
        "^(Active|Paused)$";

    private static readonly HashSet<string> ValidCodes =
    [
        Active,
        Paused,
    ];

    public static bool IsValid(string statusCode)
    {
        return ValidCodes.Contains(statusCode);
    }
}
