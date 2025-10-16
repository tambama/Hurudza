using Hurudza.Common.Utils.Exceptions;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Services.Interfaces;
using Hurudza.Data.UI.Models.ViewModels.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hurudza.Data.Services.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly HurudzaDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            HurudzaDbContext context,
            ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Inventory Management

        public async Task<List<InventoryListViewModel>> GetAllInventoryAsync()
        {
            var inventory = await _context.Inventories
                .Include(i => i.Farm)
                .Where(i => i.IsActive && !i.Deleted)
                .ToListAsync();

            return inventory.Select(MapToListViewModel).ToList();
        }

        public async Task<List<InventoryListViewModel>> GetInventoryByFarmAsync(string farmId)
        {
            if (string.IsNullOrEmpty(farmId))
                throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

            var inventory = await _context.Inventories
                .Include(i => i.Farm)
                .Where(i => i.FarmId == farmId && i.IsActive && !i.Deleted)
                .ToListAsync();

            return inventory.Select(MapToListViewModel).ToList();
        }

        public async Task<InventoryViewModel> GetInventoryByIdAsync(string inventoryId)
        {
            if (string.IsNullOrEmpty(inventoryId))
                throw new ArgumentException("Inventory ID cannot be empty", nameof(inventoryId));

            var inventory = await _context.Inventories
                .Include(i => i.Farm)
                .Include(i => i.Transactions)
                .FirstOrDefaultAsync(i => i.Id == inventoryId && i.IsActive && !i.Deleted);

            if (inventory == null)
                throw new NotFoundException($"Inventory item with ID '{inventoryId}' not found");

            return MapToViewModel(inventory);
        }

        public async Task<InventoryViewModel> CreateInventoryAsync(CreateInventoryViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Verify farm exists
            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.Id == model.FarmId && f.IsActive && !f.Deleted);

            if (farm == null)
                throw new NotFoundException($"Farm with ID '{model.FarmId}' not found");

            var inventory = MapFromCreateViewModel(model);
            inventory.QuantityInStock = model.InitialQuantity;
            inventory.LastRestockDate = DateTime.Now;

            await _context.Inventories.AddAsync(inventory);
            
            // Create initial stock transaction
            var initialTransaction = new InventoryTransaction
            {
                InventoryId = inventory.Id,
                TransactionType = TransactionType.Initial_Stock,
                Quantity = model.InitialQuantity,
                TransactionDate = DateTime.Now,
                Reason = "Initial stock entry",
                QuantityBefore = 0,
                QuantityAfter = model.InitialQuantity,
                UnitCost = model.UnitCost,
                TotalCost = model.InitialQuantity * (model.UnitCost ?? 0)
            };
            
            await _context.InventoryTransactions.AddAsync(initialTransaction);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created new inventory item '{inventory.Name}' for farm '{farm.Name}'");

            return await GetInventoryByIdAsync(inventory.Id);
        }

        public async Task<InventoryViewModel> UpdateInventoryAsync(InventoryViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == model.Id && i.IsActive && !i.Deleted);

            if (inventory == null)
                throw new NotFoundException($"Inventory item with ID '{model.Id}' not found");

            // Verify farm exists if changed
            if (inventory.FarmId != model.FarmId)
            {
                var farm = await _context.Farms
                    .FirstOrDefaultAsync(f => f.Id == model.FarmId && f.IsActive && !f.Deleted);

                if (farm == null)
                    throw new NotFoundException($"Farm with ID '{model.FarmId}' not found");
            }

            MapFromViewModelToEntity(model, inventory);
            
            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated inventory item '{inventory.Name}'");

            return await GetInventoryByIdAsync(inventory.Id);
        }

        public async Task DeleteInventoryAsync(string inventoryId)
        {
            if (string.IsNullOrEmpty(inventoryId))
                throw new ArgumentException("Inventory ID cannot be empty", nameof(inventoryId));

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == inventoryId && i.IsActive && !i.Deleted);

            if (inventory == null)
                throw new NotFoundException($"Inventory item with ID '{inventoryId}' not found");

            inventory.IsActive = false;
            inventory.Deleted = true;

            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted inventory item '{inventory.Name}'");
        }

        #endregion

        #region Stock Management

        public async Task<InventoryViewModel> UpdateStockQuantityAsync(string inventoryId, decimal newQuantity, string reason)
        {
            if (string.IsNullOrEmpty(inventoryId))
                throw new ArgumentException("Inventory ID cannot be empty", nameof(inventoryId));

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == inventoryId && i.IsActive && !i.Deleted);

            if (inventory == null)
                throw new NotFoundException($"Inventory item with ID '{inventoryId}' not found");

            var oldQuantity = inventory.QuantityInStock;
            inventory.QuantityInStock = newQuantity;

            // Create adjustment transaction
            var transaction = new InventoryTransaction
            {
                InventoryId = inventoryId,
                TransactionType = TransactionType.Adjustment,
                Quantity = Math.Abs(newQuantity - oldQuantity),
                TransactionDate = DateTime.Now,
                Reason = reason,
                QuantityBefore = oldQuantity,
                QuantityAfter = newQuantity
            };

            await _context.InventoryTransactions.AddAsync(transaction);
            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated stock quantity for '{inventory.Name}' from {oldQuantity} to {newQuantity}");

            return await GetInventoryByIdAsync(inventoryId);
        }

        public async Task<List<InventoryListViewModel>> GetLowStockItemsAsync(string? farmId = null)
        {
            var query = _context.Inventories
                .Include(i => i.Farm)
                .Where(i => i.IsActive && !i.Deleted &&
                           i.MinimumStockLevel.HasValue &&
                           i.QuantityInStock <= i.MinimumStockLevel.Value);

            if (!string.IsNullOrEmpty(farmId))
                query = query.Where(i => i.FarmId == farmId);

            var inventory = await query.ToListAsync();
            return inventory.Select(MapToListViewModel).ToList();
        }

        public async Task<List<InventoryListViewModel>> GetItemsNeedingReorderAsync(string? farmId = null)
        {
            var query = _context.Inventories
                .Include(i => i.Farm)
                .Where(i => i.IsActive && !i.Deleted &&
                           i.ReorderLevel.HasValue &&
                           i.QuantityInStock <= i.ReorderLevel.Value);

            if (!string.IsNullOrEmpty(farmId))
                query = query.Where(i => i.FarmId == farmId);

            var inventory = await query.ToListAsync();
            return inventory.Select(MapToListViewModel).ToList();
        }

        #endregion

        #region Expiry Management

        public async Task<List<InventoryListViewModel>> GetExpiredItemsAsync(string? farmId = null)
        {
            var query = _context.Inventories
                .Include(i => i.Farm)
                .Where(i => i.IsActive && !i.Deleted &&
                           i.ExpiryDate.HasValue &&
                           i.ExpiryDate.Value < DateTime.Now);

            if (!string.IsNullOrEmpty(farmId))
                query = query.Where(i => i.FarmId == farmId);

            var inventory = await query.ToListAsync();
            return inventory.Select(MapToListViewModel).ToList();
        }

        public async Task<List<InventoryListViewModel>> GetItemsNearExpiryAsync(int daysBeforeExpiry = 30, string? farmId = null)
        {
            var expiryThreshold = DateTime.Now.AddDays(daysBeforeExpiry);
            
            var query = _context.Inventories
                .Include(i => i.Farm)
                .Where(i => i.IsActive && !i.Deleted &&
                           i.ExpiryDate.HasValue &&
                           i.ExpiryDate.Value > DateTime.Now &&
                           i.ExpiryDate.Value <= expiryThreshold);

            if (!string.IsNullOrEmpty(farmId))
                query = query.Where(i => i.FarmId == farmId);

            var inventory = await query.ToListAsync();
            return inventory.Select(MapToListViewModel).ToList();
        }

        #endregion

        #region Transaction Management

        public async Task<List<InventoryTransactionListViewModel>> GetInventoryTransactionsAsync(string inventoryId)
        {
            if (string.IsNullOrEmpty(inventoryId))
                throw new ArgumentException("Inventory ID cannot be empty", nameof(inventoryId));

            var transactions = await _context.InventoryTransactions
                .Include(t => t.Inventory)
                .Include(t => t.Field)
                .Include(t => t.FieldCrop)
                    .ThenInclude(fc => fc.Crop)
                .Where(t => t.InventoryId == inventoryId && t.IsActive && !t.Deleted)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions.Select(MapToTransactionListViewModel).ToList();
        }

        public async Task<InventoryTransactionViewModel> GetTransactionByIdAsync(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                throw new ArgumentException("Transaction ID cannot be empty", nameof(transactionId));

            var transaction = await _context.InventoryTransactions
                .Include(t => t.Inventory)
                .Include(t => t.Field)
                .Include(t => t.FieldCrop)
                    .ThenInclude(fc => fc.Crop)
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.IsActive && !t.Deleted);

            if (transaction == null)
                throw new NotFoundException($"Transaction with ID '{transactionId}' not found");

            return MapToTransactionViewModel(transaction);
        }

        public async Task<InventoryTransactionViewModel> CreateTransactionAsync(CreateInventoryTransactionViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Verify inventory exists
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == model.InventoryId && i.IsActive && !i.Deleted);

            if (inventory == null)
                throw new NotFoundException($"Inventory item with ID '{model.InventoryId}' not found");

            var transaction = MapFromCreateTransactionViewModel(model);
            transaction.QuantityBefore = inventory.QuantityInStock;

            // Update inventory quantity based on transaction type
            switch (model.TransactionType)
            {
                case TransactionType.Purchase:
                case TransactionType.Return:
                case TransactionType.Initial_Stock:
                    inventory.QuantityInStock += model.Quantity;
                    inventory.LastRestockDate = model.TransactionDate;
                    break;
                case TransactionType.Usage:
                case TransactionType.Sale:
                case TransactionType.Damage:
                case TransactionType.Expiry:
                    if (inventory.QuantityInStock < model.Quantity)
                        throw new InvalidOperationException($"Insufficient stock. Available: {inventory.QuantityInStock}, Requested: {model.Quantity}");
                    inventory.QuantityInStock -= model.Quantity;
                    inventory.LastUsedDate = model.TransactionDate;
                    break;
                case TransactionType.Adjustment:
                case TransactionType.Stock_Take:
                    // Adjustment is handled separately
                    break;
            }

            transaction.QuantityAfter = inventory.QuantityInStock;
            transaction.TotalCost = model.Quantity * (model.UnitCost ?? 0);

            await _context.InventoryTransactions.AddAsync(transaction);
            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created {model.TransactionType} transaction for '{inventory.Name}'");

            return await GetTransactionByIdAsync(transaction.Id);
        }

        public async Task<InventoryTransactionViewModel> UpdateTransactionAsync(InventoryTransactionViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var transaction = await _context.InventoryTransactions
                .Include(t => t.Inventory)
                .FirstOrDefaultAsync(t => t.Id == model.Id && t.IsActive && !t.Deleted);

            if (transaction == null)
                throw new NotFoundException($"Transaction with ID '{model.Id}' not found");

            // Note: Updating transactions can be complex as it affects inventory quantities
            // This is a simplified version - in production, you'd need more sophisticated logic
            
            MapFromTransactionViewModelToEntity(model, transaction);
            
            _context.InventoryTransactions.Update(transaction);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated transaction for inventory '{transaction.Inventory?.Name}'");

            return await GetTransactionByIdAsync(transaction.Id);
        }

        public async Task DeleteTransactionAsync(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                throw new ArgumentException("Transaction ID cannot be empty", nameof(transactionId));

            var transaction = await _context.InventoryTransactions
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.IsActive && !t.Deleted);

            if (transaction == null)
                throw new NotFoundException($"Transaction with ID '{transactionId}' not found");

            transaction.IsActive = false;
            transaction.Deleted = true;

            _context.InventoryTransactions.Update(transaction);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted transaction");
        }

        #endregion

        #region Stock Movement

        public async Task<InventoryTransactionViewModel> RecordPurchaseAsync(string inventoryId, decimal quantity, 
            decimal? unitCost, string? supplierInvoiceNumber, DateTime? expiryDate, string? batchNumber, string? notes)
        {
            var model = new CreateInventoryTransactionViewModel
            {
                InventoryId = inventoryId,
                TransactionType = TransactionType.Purchase,
                Quantity = quantity,
                UnitCost = unitCost,
                TransactionDate = DateTime.Now,
                SupplierInvoiceNumber = supplierInvoiceNumber,
                ExpiryDate = expiryDate,
                BatchNumber = batchNumber,
                Notes = notes,
                Reason = "Stock purchase"
            };

            return await CreateTransactionAsync(model);
        }

        public async Task<InventoryTransactionViewModel> RecordUsageAsync(string inventoryId, decimal quantity, 
            string? fieldId, string? fieldCropId, string? reason, string? notes)
        {
            var model = new CreateInventoryTransactionViewModel
            {
                InventoryId = inventoryId,
                TransactionType = TransactionType.Usage,
                Quantity = quantity,
                TransactionDate = DateTime.Now,
                FieldId = fieldId,
                FieldCropId = fieldCropId,
                Reason = reason ?? "Stock usage",
                Notes = notes
            };

            return await CreateTransactionAsync(model);
        }

        public async Task<InventoryTransactionViewModel> RecordAdjustmentAsync(string inventoryId, decimal newQuantity, 
            string reason, string? notes)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == inventoryId && i.IsActive && !i.Deleted);

            if (inventory == null)
                throw new NotFoundException($"Inventory item with ID '{inventoryId}' not found");

            var model = new CreateInventoryTransactionViewModel
            {
                InventoryId = inventoryId,
                TransactionType = TransactionType.Adjustment,
                Quantity = Math.Abs(newQuantity - inventory.QuantityInStock),
                TransactionDate = DateTime.Now,
                Reason = reason,
                Notes = notes
            };

            var transaction = await CreateTransactionAsync(model);
            
            // Update the inventory quantity to the new value
            inventory.QuantityInStock = newQuantity;
            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();

            return transaction;
        }

        #endregion

        #region Reporting & Analytics

        public async Task<decimal> GetTotalInventoryValueAsync(string? farmId = null)
        {
            var query = _context.Inventories
                .Where(i => i.IsActive && !i.Deleted && i.UnitCost.HasValue);

            if (!string.IsNullOrEmpty(farmId))
                query = query.Where(i => i.FarmId == farmId);

            var inventory = await query.ToListAsync();
            return inventory.Sum(i => i.QuantityInStock * (i.UnitCost ?? 0));
        }

        public async Task<Dictionary<string, decimal>> GetInventoryValueByTypeAsync(string? farmId = null)
        {
            var query = _context.Inventories
                .Where(i => i.IsActive && !i.Deleted && i.UnitCost.HasValue);

            if (!string.IsNullOrEmpty(farmId))
                query = query.Where(i => i.FarmId == farmId);

            var inventory = await query.ToListAsync();
            
            return inventory
                .GroupBy(i => i.Type.ToString())
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(i => i.QuantityInStock * (i.UnitCost ?? 0))
                );
        }

        public async Task<List<InventoryTransactionListViewModel>> GetTransactionHistoryAsync(
            string? farmId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.InventoryTransactions
                .Include(t => t.Inventory)
                    .ThenInclude(i => i.Farm)
                .Include(t => t.Field)
                .Include(t => t.FieldCrop)
                    .ThenInclude(fc => fc.Crop)
                .Where(t => t.IsActive && !t.Deleted);

            if (!string.IsNullOrEmpty(farmId))
                query = query.Where(t => t.Inventory.FarmId == farmId);

            if (fromDate.HasValue)
                query = query.Where(t => t.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.TransactionDate <= toDate.Value);

            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions.Select(MapToTransactionListViewModel).ToList();
        }

        public async Task<List<InventoryAlertViewModel>> GetInventoryAlertsAsync(string? farmId = null)
        {
            var alerts = new List<InventoryAlertViewModel>();
            
            // Get low stock items
            var lowStock = await GetLowStockItemsAsync(farmId);
            alerts.AddRange(lowStock.Select(i => new InventoryAlertViewModel
            {
                Id = i.Id,
                Name = i.Name,
                FarmName = i.FarmName,
                Type = i.Type,
                AlertType = "Low Stock",
                AlertMessage = $"Stock level ({i.QuantityInStock}) is below minimum ({i.MinimumStockLevel})",
                QuantityInStock = i.QuantityInStock,
                MinimumStockLevel = i.MinimumStockLevel,
                Priority = i.NeedsReorder ? 1 : 2
            }));

            // Get items needing reorder
            var needsReorder = await GetItemsNeedingReorderAsync(farmId);
            alerts.AddRange(needsReorder.Where(i => !lowStock.Any(ls => ls.Id == i.Id)).Select(i => new InventoryAlertViewModel
            {
                Id = i.Id,
                Name = i.Name,
                FarmName = i.FarmName,
                Type = i.Type,
                AlertType = "Reorder",
                AlertMessage = $"Stock level ({i.QuantityInStock}) has reached reorder point",
                QuantityInStock = i.QuantityInStock,
                Priority = 2
            }));

            // Get expired items
            var expired = await GetExpiredItemsAsync(farmId);
            alerts.AddRange(expired.Select(i => new InventoryAlertViewModel
            {
                Id = i.Id,
                Name = i.Name,
                FarmName = i.FarmName,
                Type = i.Type,
                AlertType = "Expired",
                AlertMessage = $"Item expired on {i.ExpiryDate:yyyy-MM-dd}",
                QuantityInStock = i.QuantityInStock,
                ExpiryDate = i.ExpiryDate,
                Priority = 1
            }));

            // Get items near expiry
            var nearExpiry = await GetItemsNearExpiryAsync(30, farmId);
            alerts.AddRange(nearExpiry.Select(i => new InventoryAlertViewModel
            {
                Id = i.Id,
                Name = i.Name,
                FarmName = i.FarmName,
                Type = i.Type,
                AlertType = "Near Expiry",
                AlertMessage = $"Item will expire on {i.ExpiryDate:yyyy-MM-dd}",
                QuantityInStock = i.QuantityInStock,
                ExpiryDate = i.ExpiryDate,
                Priority = 2
            }));

            return alerts.OrderBy(a => a.Priority).ThenBy(a => a.Name).ToList();
        }

        #endregion

        #region Search & Filter

        public async Task<List<InventoryListViewModel>> SearchInventoryAsync(string searchTerm, string? farmId = null)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return new List<InventoryListViewModel>();

            var search = searchTerm.ToLower();
            
            var query = _context.Inventories
                .Include(i => i.Farm)
                .Where(i => i.IsActive && !i.Deleted &&
                           (i.Name.ToLower().Contains(search) ||
                            (i.Description != null && i.Description.ToLower().Contains(search)) ||
                            (i.SKU != null && i.SKU.ToLower().Contains(search)) ||
                            (i.Barcode != null && i.Barcode.ToLower().Contains(search)) ||
                            (i.Brand != null && i.Brand.ToLower().Contains(search)) ||
                            (i.Supplier != null && i.Supplier.ToLower().Contains(search))));

            if (!string.IsNullOrEmpty(farmId))
                query = query.Where(i => i.FarmId == farmId);

            var inventory = await query.ToListAsync();
            return inventory.Select(MapToListViewModel).ToList();
        }

        public async Task<List<InventoryListViewModel>> GetInventoryByTypeAsync(string inventoryType, string? farmId = null)
        {
            if (!Enum.TryParse<InventoryType>(inventoryType, out var type))
                throw new ArgumentException($"Invalid inventory type: {inventoryType}");

            var query = _context.Inventories
                .Include(i => i.Farm)
                .Where(i => i.IsActive && !i.Deleted && i.Type == type);

            if (!string.IsNullOrEmpty(farmId))
                query = query.Where(i => i.FarmId == farmId);

            var inventory = await query.ToListAsync();
            return inventory.Select(MapToListViewModel).ToList();
        }

        public async Task<List<InventoryListViewModel>> GetInventoryBySupplierAsync(string supplier, string? farmId = null)
        {
            if (string.IsNullOrEmpty(supplier))
                throw new ArgumentException("Supplier cannot be empty", nameof(supplier));

            var query = _context.Inventories
                .Include(i => i.Farm)
                .Where(i => i.IsActive && !i.Deleted && 
                           i.Supplier != null && i.Supplier.ToLower().Contains(supplier.ToLower()));

            if (!string.IsNullOrEmpty(farmId))
                query = query.Where(i => i.FarmId == farmId);

            var inventory = await query.ToListAsync();
            return inventory.Select(MapToListViewModel).ToList();
        }

        #endregion

        #region Mapping Helper Methods

        private InventoryListViewModel MapToListViewModel(Inventory inventory)
        {
            return new InventoryListViewModel
            {
                Id = inventory.Id,
                Name = inventory.Name,
                FarmName = inventory.Farm?.Name ?? "",
                Type = inventory.Type,
                SKU = inventory.SKU,
                Brand = inventory.Brand,
                QuantityInStock = inventory.QuantityInStock,
                UnitOfMeasure = inventory.UnitOfMeasure,
                MinimumStockLevel = inventory.MinimumStockLevel,
                ExpiryDate = inventory.ExpiryDate,
                StorageLocation = inventory.StorageLocation,
                TotalValue = inventory.QuantityInStock * (inventory.UnitCost ?? 0),
                IsLowStock = inventory.MinimumStockLevel.HasValue && inventory.QuantityInStock <= inventory.MinimumStockLevel.Value,
                NeedsReorder = inventory.ReorderLevel.HasValue && inventory.QuantityInStock <= inventory.ReorderLevel.Value,
                IsExpired = inventory.ExpiryDate.HasValue && inventory.ExpiryDate.Value < DateTime.Now,
                IsNearExpiry = inventory.ExpiryDate.HasValue && inventory.ExpiryDate.Value <= DateTime.Now.AddDays(30) && inventory.ExpiryDate.Value > DateTime.Now
            };
        }

        private InventoryViewModel MapToViewModel(Inventory inventory)
        {
            return new InventoryViewModel
            {
                Id = inventory.Id,
                Name = inventory.Name,
                Description = inventory.Description,
                Type = inventory.Type,
                FarmId = inventory.FarmId,
                FarmName = inventory.Farm?.Name,
                SKU = inventory.SKU,
                Barcode = inventory.Barcode,
                Brand = inventory.Brand,
                Supplier = inventory.Supplier,
                QuantityInStock = inventory.QuantityInStock,
                UnitOfMeasure = inventory.UnitOfMeasure,
                MinimumStockLevel = inventory.MinimumStockLevel,
                ReorderLevel = inventory.ReorderLevel,
                ReorderQuantity = inventory.ReorderQuantity,
                UnitCost = inventory.UnitCost,
                SellingPrice = inventory.SellingPrice,
                ExpiryDate = inventory.ExpiryDate,
                ManufactureDate = inventory.ManufactureDate,
                BatchNumber = inventory.BatchNumber,
                StorageLocation = inventory.StorageLocation,
                StorageConditions = inventory.StorageConditions,
                Notes = inventory.Notes,
                IsPerishable = inventory.IsPerishable,
                RequiresSpecialHandling = inventory.RequiresSpecialHandling,
                SpecialHandlingInstructions = inventory.SpecialHandlingInstructions,
                LastRestockDate = inventory.LastRestockDate,
                LastUsedDate = inventory.LastUsedDate,
                Transactions = inventory.Transactions?.Select(MapToTransactionViewModel).ToList() ?? new List<InventoryTransactionViewModel>()
            };
        }

        private Inventory MapFromCreateViewModel(CreateInventoryViewModel model)
        {
            return new Inventory
            {
                Name = model.Name,
                Description = model.Description,
                Type = model.Type,
                FarmId = model.FarmId,
                SKU = model.SKU,
                Barcode = model.Barcode,
                Brand = model.Brand,
                Supplier = model.Supplier,
                UnitOfMeasure = model.UnitOfMeasure,
                MinimumStockLevel = model.MinimumStockLevel,
                ReorderLevel = model.ReorderLevel,
                ReorderQuantity = model.ReorderQuantity,
                UnitCost = model.UnitCost,
                SellingPrice = model.SellingPrice,
                ExpiryDate = model.ExpiryDate,
                ManufactureDate = model.ManufactureDate,
                BatchNumber = model.BatchNumber,
                StorageLocation = model.StorageLocation,
                StorageConditions = model.StorageConditions,
                Notes = model.Notes,
                IsPerishable = model.IsPerishable,
                RequiresSpecialHandling = model.RequiresSpecialHandling,
                SpecialHandlingInstructions = model.SpecialHandlingInstructions
            };
        }

        private void MapFromViewModelToEntity(InventoryViewModel model, Inventory inventory)
        {
            inventory.Name = model.Name;
            inventory.Description = model.Description;
            inventory.Type = model.Type;
            inventory.FarmId = model.FarmId;
            inventory.SKU = model.SKU;
            inventory.Barcode = model.Barcode;
            inventory.Brand = model.Brand;
            inventory.Supplier = model.Supplier;
            inventory.QuantityInStock = model.QuantityInStock;
            inventory.UnitOfMeasure = model.UnitOfMeasure;
            inventory.MinimumStockLevel = model.MinimumStockLevel;
            inventory.ReorderLevel = model.ReorderLevel;
            inventory.ReorderQuantity = model.ReorderQuantity;
            inventory.UnitCost = model.UnitCost;
            inventory.SellingPrice = model.SellingPrice;
            inventory.ExpiryDate = model.ExpiryDate;
            inventory.ManufactureDate = model.ManufactureDate;
            inventory.BatchNumber = model.BatchNumber;
            inventory.StorageLocation = model.StorageLocation;
            inventory.StorageConditions = model.StorageConditions;
            inventory.Notes = model.Notes;
            inventory.IsPerishable = model.IsPerishable;
            inventory.RequiresSpecialHandling = model.RequiresSpecialHandling;
            inventory.SpecialHandlingInstructions = model.SpecialHandlingInstructions;
        }

        private InventoryTransactionListViewModel MapToTransactionListViewModel(InventoryTransaction transaction)
        {
            return new InventoryTransactionListViewModel
            {
                Id = transaction.Id,
                InventoryName = transaction.Inventory?.Name ?? "",
                TransactionType = transaction.TransactionType,
                Quantity = transaction.Quantity,
                TotalCost = transaction.TotalCost,
                TransactionDate = transaction.TransactionDate,
                ReferenceNumber = transaction.ReferenceNumber,
                FieldName = transaction.Field?.Name,
                CropName = transaction.FieldCrop?.Crop?.Name,
                QuantityBefore = transaction.QuantityBefore,
                QuantityAfter = transaction.QuantityAfter,
                PerformedBy = transaction.ReceivedBy ?? transaction.ApprovedBy
            };
        }

        private InventoryTransactionViewModel MapToTransactionViewModel(InventoryTransaction transaction)
        {
            return new InventoryTransactionViewModel
            {
                Id = transaction.Id,
                InventoryId = transaction.InventoryId,
                InventoryName = transaction.Inventory?.Name,
                TransactionType = transaction.TransactionType,
                Quantity = transaction.Quantity,
                UnitCost = transaction.UnitCost,
                TotalCost = transaction.TotalCost,
                TransactionDate = transaction.TransactionDate,
                Reason = transaction.Reason,
                ReferenceNumber = transaction.ReferenceNumber,
                SupplierInvoiceNumber = transaction.SupplierInvoiceNumber,
                ReceivedBy = transaction.ReceivedBy,
                ApprovedBy = transaction.ApprovedBy,
                FieldId = transaction.FieldId,
                FieldName = transaction.Field?.Name,
                FieldCropId = transaction.FieldCropId,
                CropName = transaction.FieldCrop?.Crop?.Name,
                BatchNumber = transaction.BatchNumber,
                ExpiryDate = transaction.ExpiryDate,
                QuantityBefore = transaction.QuantityBefore,
                QuantityAfter = transaction.QuantityAfter,
                Notes = transaction.Notes
            };
        }

        private InventoryTransaction MapFromCreateTransactionViewModel(CreateInventoryTransactionViewModel model)
        {
            return new InventoryTransaction
            {
                InventoryId = model.InventoryId,
                TransactionType = model.TransactionType,
                Quantity = model.Quantity,
                UnitCost = model.UnitCost,
                TransactionDate = model.TransactionDate,
                Reason = model.Reason,
                ReferenceNumber = model.ReferenceNumber,
                SupplierInvoiceNumber = model.SupplierInvoiceNumber,
                ReceivedBy = model.ReceivedBy,
                ApprovedBy = model.ApprovedBy,
                FieldId = model.FieldId,
                FieldCropId = model.FieldCropId,
                BatchNumber = model.BatchNumber,
                ExpiryDate = model.ExpiryDate,
                Notes = model.Notes
            };
        }

        private void MapFromTransactionViewModelToEntity(InventoryTransactionViewModel model, InventoryTransaction transaction)
        {
            transaction.InventoryId = model.InventoryId;
            transaction.TransactionType = model.TransactionType;
            transaction.Quantity = model.Quantity;
            transaction.UnitCost = model.UnitCost;
            transaction.TotalCost = model.TotalCost;
            transaction.TransactionDate = model.TransactionDate;
            transaction.Reason = model.Reason;
            transaction.ReferenceNumber = model.ReferenceNumber;
            transaction.SupplierInvoiceNumber = model.SupplierInvoiceNumber;
            transaction.ReceivedBy = model.ReceivedBy;
            transaction.ApprovedBy = model.ApprovedBy;
            transaction.FieldId = model.FieldId;
            transaction.FieldCropId = model.FieldCropId;
            transaction.BatchNumber = model.BatchNumber;
            transaction.ExpiryDate = model.ExpiryDate;
            transaction.QuantityBefore = model.QuantityBefore;
            transaction.QuantityAfter = model.QuantityAfter;
            transaction.Notes = model.Notes;
        }

        #endregion
    }
}