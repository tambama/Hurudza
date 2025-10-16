using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Data.Services.Interfaces;

public interface IInventoryService
{
    // Inventory Management
    Task<List<InventoryListViewModel>> GetAllInventoryAsync();
    Task<List<InventoryListViewModel>> GetInventoryByFarmAsync(string farmId);
    Task<InventoryViewModel> GetInventoryByIdAsync(string inventoryId);
    Task<InventoryViewModel> CreateInventoryAsync(CreateInventoryViewModel model);
    Task<InventoryViewModel> UpdateInventoryAsync(InventoryViewModel model);
    Task DeleteInventoryAsync(string inventoryId);
    
    // Stock Management
    Task<InventoryViewModel> UpdateStockQuantityAsync(string inventoryId, decimal newQuantity, string reason);
    Task<List<InventoryListViewModel>> GetLowStockItemsAsync(string? farmId = null);
    Task<List<InventoryListViewModel>> GetItemsNeedingReorderAsync(string? farmId = null);
    
    // Expiry Management
    Task<List<InventoryListViewModel>> GetExpiredItemsAsync(string? farmId = null);
    Task<List<InventoryListViewModel>> GetItemsNearExpiryAsync(int daysBeforeExpiry = 30, string? farmId = null);
    
    // Transaction Management
    Task<List<InventoryTransactionListViewModel>> GetInventoryTransactionsAsync(string inventoryId);
    Task<InventoryTransactionViewModel> GetTransactionByIdAsync(string transactionId);
    Task<InventoryTransactionViewModel> CreateTransactionAsync(CreateInventoryTransactionViewModel model);
    Task<InventoryTransactionViewModel> UpdateTransactionAsync(InventoryTransactionViewModel model);
    Task DeleteTransactionAsync(string transactionId);
    
    // Stock Movement
    Task<InventoryTransactionViewModel> RecordPurchaseAsync(string inventoryId, decimal quantity, decimal? unitCost, 
        string? supplierInvoiceNumber, DateTime? expiryDate, string? batchNumber, string? notes);
    Task<InventoryTransactionViewModel> RecordUsageAsync(string inventoryId, decimal quantity, 
        string? fieldId, string? fieldCropId, string? reason, string? notes);
    Task<InventoryTransactionViewModel> RecordAdjustmentAsync(string inventoryId, decimal newQuantity, 
        string reason, string? notes);
    
    // Reporting & Analytics
    Task<decimal> GetTotalInventoryValueAsync(string? farmId = null);
    Task<Dictionary<string, decimal>> GetInventoryValueByTypeAsync(string? farmId = null);
    Task<List<InventoryTransactionListViewModel>> GetTransactionHistoryAsync(
        string? farmId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<InventoryAlertViewModel>> GetInventoryAlertsAsync(string? farmId = null);
    
    // Search & Filter
    Task<List<InventoryListViewModel>> SearchInventoryAsync(string searchTerm, string? farmId = null);
    Task<List<InventoryListViewModel>> GetInventoryByTypeAsync(string inventoryType, string? farmId = null);
    Task<List<InventoryListViewModel>> GetInventoryBySupplierAsync(string supplier, string? farmId = null);
}