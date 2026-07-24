namespace BuddyErp.Api.DTOs.Formations;

public static class FormationCodes
{
    private static readonly IReadOnlyDictionary<string, HashSet<string>> Slots =
        new Dictionary<string, HashSet<string>>(StringComparer.Ordinal)
        {
            ["4-2-3-1"] =
            [
                "goalkeeper", "leftBack", "leftCenterBack",
                "rightCenterBack", "rightBack",
                "leftDefensiveMidfielder", "rightDefensiveMidfielder",
                "leftWinger", "attackingMidfielder", "rightWinger",
                "striker",
            ],
            ["4-1-2-3"] =
            [
                "goalkeeper", "leftBack", "leftCenterBack",
                "rightCenterBack", "rightBack", "defensiveMidfielder",
                "leftCentralMidfielder", "rightCentralMidfielder",
                "leftForward", "centerForward", "rightForward",
            ],
            ["4-5-1"] =
            [
                "goalkeeper", "leftBack", "leftCenterBack",
                "rightCenterBack", "rightBack", "leftMidfielder",
                "leftCentralMidfielder", "centerMidfielder",
                "rightCentralMidfielder", "rightMidfielder", "striker",
            ],
            ["4-3-3"] =
            [
                "goalkeeper", "leftBack", "leftCenterBack",
                "rightCenterBack", "rightBack", "leftMidfielder",
                "centerMidfielder", "rightMidfielder",
                "leftForward", "centerForward", "rightForward",
            ],
        };

    public static bool IsValid(string formationCode)
    {
        return Slots.ContainsKey(formationCode);
    }

    public static bool IsValidSlot(string formationCode, string slotCode)
    {
        return Slots.TryGetValue(formationCode, out var slots) &&
            slots.Contains(slotCode);
    }
}
