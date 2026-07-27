namespace BuddyErp.Api.Data.Entities;

// 기존 구매 데이터를 보존하기 위한 DB 매핑입니다.
// 현재 물품 기능에서는 이 엔티티를 조회하거나 수정하지 않습니다.
public sealed class InventoryPurchase
{
    public long PurchaseId { get; set; }
    public required string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public bool IsPurchased { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
