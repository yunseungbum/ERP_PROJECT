using BuddyErp.Api.DTOs.Inventory;

namespace BuddyErp.Api.Services.Inventory;

public interface IInventoryService
{
    Task<IReadOnlyList<InventoryItemResponse>> GetItemsAsync(
        CancellationToken cancellationToken = default);

    Task<InventoryItemResponse> CreateItemAsync(
        InventoryItemSaveRequest request,
        CancellationToken cancellationToken = default);

    Task<InventoryItemResponse?> UpdateItemAsync(
        long inventoryItemId,
        InventoryItemSaveRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteItemAsync(
        long inventoryItemId,
        CancellationToken cancellationToken = default);
}
