namespace BuddyErp.Api.DTOs.Inventory;

public sealed record InventoryItemResponse(
    long InventoryItemId,
    string ItemName,
    int Quantity);
