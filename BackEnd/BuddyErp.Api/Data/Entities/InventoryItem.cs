namespace BuddyErp.Api.Data.Entities;

public sealed class InventoryItem
{
    public long InventoryItemId { get; set; }
    public required string ItemName { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
