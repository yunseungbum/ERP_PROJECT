namespace BuddyErp.Api.DTOs.Members;

public static class MemberPositionCodes
{
    public const string Goalkeeper = "Goalkeeper";
    public const string WingBack = "WingBack";
    public const string CenterBack = "CenterBack";
    public const string DefensiveMidfielder = "DefensiveMidfielder";
    public const string CentralMidfielder = "CentralMidfielder";
    public const string AttackingMidfielder = "AttackingMidfielder";
    public const string Winger = "Winger";
    public const string Striker = "Striker";

    public const string ValidationPattern =
        "^(Goalkeeper|WingBack|CenterBack|DefensiveMidfielder|" +
        "CentralMidfielder|AttackingMidfielder|Winger|Striker)$";

    private static readonly HashSet<string> ValidCodes =
    [
        Goalkeeper,
        WingBack,
        CenterBack,
        DefensiveMidfielder,
        CentralMidfielder,
        AttackingMidfielder,
        Winger,
        Striker,
    ];

    public static bool IsValid(string positionCode)
    {
        return ValidCodes.Contains(positionCode);
    }
}
