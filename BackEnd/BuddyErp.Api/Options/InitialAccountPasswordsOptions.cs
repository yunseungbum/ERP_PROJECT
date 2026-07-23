namespace BuddyErp.Api.Options;

public sealed class InitialAccountPasswordsOptions
{
    public const string SectionName = "InitialAccountPasswords";

    public string President { get; init; } = string.Empty;
    public string Director { get; init; } = string.Empty;
    public string Coach { get; init; } = string.Empty;
    public string Treasurer { get; init; } = string.Empty;
    public string InventoryManager { get; init; } = string.Empty;
    public string Member { get; init; } = string.Empty;
    public string Guest { get; init; } = string.Empty;
}
