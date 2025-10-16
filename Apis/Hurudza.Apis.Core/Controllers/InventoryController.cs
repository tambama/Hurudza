using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using Asp.Versioning;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Data;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Services.Interfaces;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;
    private readonly UserManager<ApplicationUser> _userManager;

    public InventoryController(IInventoryService inventoryService, 
        UserManager<ApplicationUser> userManager)
    {
        _inventoryService = inventoryService;
        _userManager = userManager;
    }

    #region Inventory Management

    [HttpGet(Name = nameof(GetInventory))]
    public async Task<IActionResult> GetInventory()
    {
        try
        {
            var inventory = await _inventoryService.GetAllInventoryAsync().ConfigureAwait(false);
            return Ok(new ApiOkResponse(inventory));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading inventory: {ex.Message}"));
        }
    }

    [HttpGet("{farmId}", Name = nameof(GetInventoryByFarm))]
    public async Task<IActionResult> GetInventoryByFarm(string farmId)
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryByFarmAsync(farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(inventory));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading inventory: {ex.Message}"));
        }
    }

    [HttpGet("{id}", Name = nameof(GetInventoryDetails))]
    public async Task<IActionResult> GetInventoryDetails(string id)
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryByIdAsync(id).ConfigureAwait(false);
            return Ok(inventory == null
                ? new ApiResponse((int)HttpStatusCode.NotFound, "Inventory item not found")
                : new ApiOkResponse(inventory));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading inventory item: {ex.Message}"));
        }
    }

    [HttpPost(Name = nameof(CreateInventory))]
    public async Task<IActionResult> CreateInventory([FromBody] CreateInventoryViewModel model)
    {
        try
        {
            var inventory = await _inventoryService.CreateInventoryAsync(model).ConfigureAwait(false);
            return Ok(new ApiOkResponse(inventory, "Inventory item successfully created"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error creating inventory item: {ex.Message}"));
        }
    }

    [HttpPut("{id}", Name = nameof(UpdateInventory))]
    public async Task<IActionResult> UpdateInventory(string id, [FromBody] InventoryViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var inventory = await _inventoryService.UpdateInventoryAsync(model).ConfigureAwait(false);
            if (inventory == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Inventory item not found"));
            }
            return Ok(new ApiOkResponse(inventory, $"{inventory.Name} successfully updated"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error updating inventory item: {ex.Message}"));
        }
    }

    [HttpDelete("{id}", Name = nameof(DeleteInventory))]
    public async Task<IActionResult> DeleteInventory(string id)
    {
        try
        {
            await _inventoryService.DeleteInventoryAsync(id).ConfigureAwait(false);
            return Ok(new ApiOkResponse("Inventory item was deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error deleting inventory item: {ex.Message}"));
        }
    }

    #endregion

    #region Stock Management

    [HttpGet("low-stock", Name = nameof(GetLowStockItems))]
    public async Task<IActionResult> GetLowStockItems([FromQuery] string? farmId = null)
    {
        try
        {
            var items = await _inventoryService.GetLowStockItemsAsync(farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading low stock items: {ex.Message}"));
        }
    }

    [HttpGet("reorder-needed", Name = nameof(GetItemsNeedingReorder))]
    public async Task<IActionResult> GetItemsNeedingReorder([FromQuery] string? farmId = null)
    {
        try
        {
            var items = await _inventoryService.GetItemsNeedingReorderAsync(farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading items needing reorder: {ex.Message}"));
        }
    }

    [HttpPost("{id}/update-stock", Name = nameof(UpdateStock))]
    public async Task<IActionResult> UpdateStock(string id, [FromBody] UpdateStockRequest request)
    {
        try
        {
            var inventory = await _inventoryService.UpdateStockQuantityAsync(id, request.NewQuantity, request.Reason).ConfigureAwait(false);
            return Ok(new ApiOkResponse(inventory, "Stock quantity updated successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error updating stock: {ex.Message}"));
        }
    }

    #endregion

    #region Expiry Management

    [HttpGet("expired", Name = nameof(GetExpiredItems))]
    public async Task<IActionResult> GetExpiredItems([FromQuery] string? farmId = null)
    {
        try
        {
            var items = await _inventoryService.GetExpiredItemsAsync(farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading expired items: {ex.Message}"));
        }
    }

    [HttpGet("near-expiry", Name = nameof(GetItemsNearExpiry))]
    public async Task<IActionResult> GetItemsNearExpiry([FromQuery] int daysBeforeExpiry = 30, [FromQuery] string? farmId = null)
    {
        try
        {
            var items = await _inventoryService.GetItemsNearExpiryAsync(daysBeforeExpiry, farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading items near expiry: {ex.Message}"));
        }
    }

    #endregion

    #region Transaction Management

    [HttpGet("{inventoryId}/transactions", Name = nameof(GetInventoryTransactions))]
    public async Task<IActionResult> GetInventoryTransactions(string inventoryId)
    {
        try
        {
            var transactions = await _inventoryService.GetInventoryTransactionsAsync(inventoryId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(transactions));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading transactions: {ex.Message}"));
        }
    }

    [HttpGet("transaction/{id}", Name = nameof(GetTransactionDetails))]
    public async Task<IActionResult> GetTransactionDetails(string id)
    {
        try
        {
            var transaction = await _inventoryService.GetTransactionByIdAsync(id).ConfigureAwait(false);
            return Ok(transaction == null
                ? new ApiResponse((int)HttpStatusCode.NotFound, "Transaction not found")
                : new ApiOkResponse(transaction));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading transaction: {ex.Message}"));
        }
    }

    [HttpPost("transaction", Name = nameof(CreateTransaction))]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateInventoryTransactionViewModel model)
    {
        try
        {
            var transaction = await _inventoryService.CreateTransactionAsync(model).ConfigureAwait(false);
            return Ok(new ApiOkResponse(transaction, "Transaction successfully recorded"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error creating transaction: {ex.Message}"));
        }
    }

    [HttpPost("{id}/purchase", Name = nameof(RecordPurchase))]
    public async Task<IActionResult> RecordPurchase(string id, [FromBody] RecordPurchaseRequest request)
    {
        try
        {
            var transaction = await _inventoryService.RecordPurchaseAsync(
                id, 
                request.Quantity, 
                request.UnitCost,
                request.SupplierInvoiceNumber,
                request.ExpiryDate,
                request.BatchNumber,
                request.Notes
            ).ConfigureAwait(false);
            return Ok(new ApiOkResponse(transaction, "Purchase successfully recorded"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error recording purchase: {ex.Message}"));
        }
    }

    [HttpPost("{id}/usage", Name = nameof(RecordUsage))]
    public async Task<IActionResult> RecordUsage(string id, [FromBody] RecordUsageRequest request)
    {
        try
        {
            var transaction = await _inventoryService.RecordUsageAsync(
                id,
                request.Quantity,
                request.FieldId,
                request.FieldCropId,
                request.Reason,
                request.Notes
            ).ConfigureAwait(false);
            return Ok(new ApiOkResponse(transaction, "Usage successfully recorded"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error recording usage: {ex.Message}"));
        }
    }

    #endregion

    #region Reporting & Analytics

    [HttpGet("alerts", Name = nameof(GetInventoryAlerts))]
    public async Task<IActionResult> GetInventoryAlerts([FromQuery] string? farmId = null)
    {
        try
        {
            var alerts = await _inventoryService.GetInventoryAlertsAsync(farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(alerts));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading inventory alerts: {ex.Message}"));
        }
    }

    [HttpGet("total-value", Name = nameof(GetTotalInventoryValue))]
    public async Task<IActionResult> GetTotalInventoryValue([FromQuery] string? farmId = null)
    {
        try
        {
            var value = await _inventoryService.GetTotalInventoryValueAsync(farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(new { TotalValue = value }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error calculating inventory value: {ex.Message}"));
        }
    }

    [HttpGet("value-by-type", Name = nameof(GetInventoryValueByType))]
    public async Task<IActionResult> GetInventoryValueByType([FromQuery] string? farmId = null)
    {
        try
        {
            var values = await _inventoryService.GetInventoryValueByTypeAsync(farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(values));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error calculating inventory values: {ex.Message}"));
        }
    }

    #endregion

    #region Search & Filter

    [HttpGet("search", Name = nameof(SearchInventory))]
    public async Task<IActionResult> SearchInventory([FromQuery] string searchTerm, [FromQuery] string? farmId = null)
    {
        try
        {
            var items = await _inventoryService.SearchInventoryAsync(searchTerm, farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error searching inventory: {ex.Message}"));
        }
    }

    [HttpGet("by-type/{type}", Name = nameof(GetInventoryByType))]
    public async Task<IActionResult> GetInventoryByType(string type, [FromQuery] string? farmId = null)
    {
        try
        {
            var items = await _inventoryService.GetInventoryByTypeAsync(type, farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(items));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading inventory by type: {ex.Message}"));
        }
    }

    #endregion
}

// Request DTOs
public class UpdateStockRequest
{
    public decimal NewQuantity { get; set; }
    public string Reason { get; set; }
}

public class RecordPurchaseRequest
{
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? SupplierInvoiceNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }
}

public class RecordUsageRequest
{
    public decimal Quantity { get; set; }
    public string? FieldId { get; set; }
    public string? FieldCropId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}