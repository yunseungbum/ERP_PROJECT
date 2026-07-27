using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.DTOs.Inventory;
using Microsoft.EntityFrameworkCore;

namespace BuddyErp.Api.Services.Inventory;

public sealed class InventoryService(AppDbContext dbContext)
    : IInventoryService
{
    public async Task<IReadOnlyList<InventoryItemResponse>> GetItemsAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.InventoryItems
            .AsNoTracking()
            .OrderBy(item => item.InventoryItemId)
            .Select(item => new InventoryItemResponse(
                item.InventoryItemId,
                item.ItemName,
                item.Quantity))
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryItemResponse> CreateItemAsync(
        InventoryItemSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var itemName = ValidateAndNormalizeName(request.ItemName);
        await EnsureUniqueNameAsync(itemName, null, cancellationToken);
        var now = DateTime.UtcNow;
        var item = new InventoryItem
        {
            ItemName = itemName,
            Quantity = request.Quantity,
            CreatedAt = now,
            UpdatedAt = now,
        };

        dbContext.InventoryItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(item);
    }

    public async Task<InventoryItemResponse?> UpdateItemAsync(
        long inventoryItemId,
        InventoryItemSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var item = await dbContext.InventoryItems.SingleOrDefaultAsync(
            item => item.InventoryItemId == inventoryItemId,
            cancellationToken);

        if (item is null)
        {
            return null;
        }

        var itemName = ValidateAndNormalizeName(request.ItemName);
        await EnsureUniqueNameAsync(
            itemName,
            inventoryItemId,
            cancellationToken);

        item.ItemName = itemName;
        item.Quantity = request.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(item);
    }

    public async Task<bool> DeleteItemAsync(
        long inventoryItemId,
        CancellationToken cancellationToken = default)
    {
        var item = await dbContext.InventoryItems.SingleOrDefaultAsync(
            item => item.InventoryItemId == inventoryItemId,
            cancellationToken);

        if (item is null)
        {
            return false;
        }

        dbContext.InventoryItems.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureUniqueNameAsync(
        string itemName,
        long? excludedItemId,
        CancellationToken cancellationToken)
    {
        var duplicateExists = await dbContext.InventoryItems.AnyAsync(
            item =>
                item.ItemName == itemName &&
                item.InventoryItemId != excludedItemId,
            cancellationToken);

        if (duplicateExists)
        {
            throw new ArgumentException("같은 품명의 물품이 이미 있습니다.");
        }
    }

    private static string ValidateAndNormalizeName(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
        {
            throw new ArgumentException("품명을 입력해 주세요.");
        }

        return itemName.Trim();
    }

    private static InventoryItemResponse ToResponse(InventoryItem item)
    {
        return new InventoryItemResponse(
            item.InventoryItemId,
            item.ItemName,
            item.Quantity);
    }
}
