using System.ComponentModel.DataAnnotations;

namespace BuddyErp.Api.DTOs.Inventory;

public sealed record InventoryItemSaveRequest(
    [Required, MaxLength(100)] string ItemName,
    [Range(0, int.MaxValue)] int Quantity);
