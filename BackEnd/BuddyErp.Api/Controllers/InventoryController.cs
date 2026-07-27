using BuddyErp.Api.DTOs.Inventory;
using BuddyErp.Api.Services.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuddyErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/inventory")]
public sealed class InventoryController(
    IInventoryService inventoryService) : ControllerBase
{
    private const string InventoryWriters =
        "President,Director,Coach,Treasurer,InventoryManager";

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InventoryItemResponse>>> GetItems(
        CancellationToken cancellationToken)
    {
        return Ok(await inventoryService.GetItemsAsync(cancellationToken));
    }

    [Authorize(Roles = InventoryWriters)]
    [HttpPost]
    public async Task<ActionResult<InventoryItemResponse>> CreateItem(
        InventoryItemSaveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await inventoryService.CreateItemAsync(
                request,
                cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
    }

    [Authorize(Roles = InventoryWriters)]
    [HttpPut("{inventoryItemId:long}")]
    public async Task<ActionResult<InventoryItemResponse>> UpdateItem(
        long inventoryItemId,
        InventoryItemSaveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await inventoryService.UpdateItemAsync(
                inventoryItemId,
                request,
                cancellationToken);
            return item is null ? NotFound() : Ok(item);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Detail = exception.Message });
        }
    }

    [Authorize(Roles = InventoryWriters)]
    [HttpDelete("{inventoryItemId:long}")]
    public async Task<IActionResult> DeleteItem(
        long inventoryItemId,
        CancellationToken cancellationToken)
    {
        var deleted = await inventoryService.DeleteItemAsync(
            inventoryItemId,
            cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
