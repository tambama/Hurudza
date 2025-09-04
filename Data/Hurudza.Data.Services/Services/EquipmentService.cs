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
    /// <summary>
    /// Service for managing equipment and equipment maintenance operations
    /// </summary>
    public class EquipmentService : IEquipmentService
    {
        private readonly HurudzaDbContext _context;
        private readonly ILogger<EquipmentService> _logger;

        public EquipmentService(
            HurudzaDbContext context,
            ILogger<EquipmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Equipment Management

        /// <summary>
        /// Gets all active equipment
        /// </summary>
        public async Task<List<EquipmentListViewModel>> GetAllEquipmentAsync()
        {
            var equipment = await _context.Equipment
                .Include(e => e.Farm)
                .Where(e => e.IsActive && !e.Deleted)
                .ToListAsync();

            return equipment.Select(MapToListViewModel).ToList();
        }

        /// <summary>
        /// Gets all equipment for a specific farm
        /// </summary>
        public async Task<List<EquipmentListViewModel>> GetEquipmentByFarmAsync(string farmId)
        {
            if (string.IsNullOrEmpty(farmId))
                throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

            var equipment = await _context.Equipment
                .Include(e => e.Farm)
                .Where(e => e.FarmId == farmId && e.IsActive && !e.Deleted)
                .ToListAsync();

            return equipment.Select(MapToListViewModel).ToList();
        }

        /// <summary>
        /// Gets equipment by ID with full details
        /// </summary>
        public async Task<EquipmentViewModel> GetEquipmentByIdAsync(string equipmentId)
        {
            if (string.IsNullOrEmpty(equipmentId))
                throw new ArgumentException("Equipment ID cannot be empty", nameof(equipmentId));

            var equipment = await _context.Equipment
                .Include(e => e.Farm)
                .Include(e => e.MaintenanceRecords)
                .FirstOrDefaultAsync(e => e.Id == equipmentId && e.IsActive && !e.Deleted);

            if (equipment == null)
                throw new NotFoundException($"Equipment with ID '{equipmentId}' not found");

            return MapToViewModel(equipment);
        }

        /// <summary>
        /// Creates new equipment
        /// </summary>
        public async Task<EquipmentViewModel> CreateEquipmentAsync(CreateEquipmentViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Verify farm exists
            var farm = await _context.Farms
                .FirstOrDefaultAsync(f => f.Id == model.FarmId && f.IsActive && !f.Deleted);

            if (farm == null)
                throw new NotFoundException($"Farm with ID '{model.FarmId}' not found");

            var equipment = MapFromCreateViewModel(model);

            await _context.Equipment.AddAsync(equipment);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created new equipment '{equipment.Name}' for farm '{farm.Name}'");

            return await GetEquipmentByIdAsync(equipment.Id);
        }

        /// <summary>
        /// Updates existing equipment
        /// </summary>
        public async Task<EquipmentViewModel> UpdateEquipmentAsync(EquipmentViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.Id == model.Id && e.IsActive && !e.Deleted);

            if (equipment == null)
                throw new NotFoundException($"Equipment with ID '{model.Id}' not found");

            // Verify farm exists if changed
            if (equipment.FarmId != model.FarmId)
            {
                var farm = await _context.Farms
                    .FirstOrDefaultAsync(f => f.Id == model.FarmId && f.IsActive && !f.Deleted);

                if (farm == null)
                    throw new NotFoundException($"Farm with ID '{model.FarmId}' not found");
            }

            MapFromViewModelToEntity(model, equipment);
            
            _context.Equipment.Update(equipment);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated equipment '{equipment.Name}'");

            return await GetEquipmentByIdAsync(equipment.Id);
        }

        /// <summary>
        /// Deletes equipment (soft delete)
        /// </summary>
        public async Task DeleteEquipmentAsync(string equipmentId)
        {
            if (string.IsNullOrEmpty(equipmentId))
                throw new ArgumentException("Equipment ID cannot be empty", nameof(equipmentId));

            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.Id == equipmentId && e.IsActive && !e.Deleted);

            if (equipment == null)
                throw new NotFoundException($"Equipment with ID '{equipmentId}' not found");

            equipment.IsActive = false;
            equipment.Deleted = true;

            _context.Equipment.Update(equipment);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted equipment '{equipment.Name}'");
        }

        #endregion

        #region Equipment Status & Condition Management

        /// <summary>
        /// Updates equipment status
        /// </summary>
        public async Task<EquipmentViewModel> UpdateEquipmentStatusAsync(string equipmentId, string status)
        {
            if (string.IsNullOrEmpty(equipmentId))
                throw new ArgumentException("Equipment ID cannot be empty", nameof(equipmentId));

            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Status cannot be empty", nameof(status));

            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.Id == equipmentId && e.IsActive && !e.Deleted);

            if (equipment == null)
                throw new NotFoundException($"Equipment with ID '{equipmentId}' not found");

            if (Enum.TryParse<EquipmentStatus>(status, out var equipmentStatus))
            {
                equipment.Status = equipmentStatus;
                _context.Equipment.Update(equipment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated equipment '{equipment.Name}' status to '{status}'");
            }
            else
            {
                throw new ArgumentException($"Invalid equipment status: {status}");
            }

            return await GetEquipmentByIdAsync(equipmentId);
        }

        /// <summary>
        /// Updates equipment condition
        /// </summary>
        public async Task<EquipmentViewModel> UpdateEquipmentConditionAsync(string equipmentId, string condition)
        {
            if (string.IsNullOrEmpty(equipmentId))
                throw new ArgumentException("Equipment ID cannot be empty", nameof(equipmentId));

            if (string.IsNullOrEmpty(condition))
                throw new ArgumentException("Condition cannot be empty", nameof(condition));

            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.Id == equipmentId && e.IsActive && !e.Deleted);

            if (equipment == null)
                throw new NotFoundException($"Equipment with ID '{equipmentId}' not found");

            if (Enum.TryParse<EquipmentCondition>(condition, out var equipmentCondition))
            {
                equipment.Condition = equipmentCondition;
                _context.Equipment.Update(equipment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated equipment '{equipment.Name}' condition to '{condition}'");
            }
            else
            {
                throw new ArgumentException($"Invalid equipment condition: {condition}");
            }

            return await GetEquipmentByIdAsync(equipmentId);
        }

        /// <summary>
        /// Updates operating hours
        /// </summary>
        public async Task<EquipmentViewModel> UpdateOperatingHoursAsync(string equipmentId, int operatingHours)
        {
            if (string.IsNullOrEmpty(equipmentId))
                throw new ArgumentException("Equipment ID cannot be empty", nameof(equipmentId));

            if (operatingHours < 0)
                throw new ArgumentException("Operating hours cannot be negative", nameof(operatingHours));

            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.Id == equipmentId && e.IsActive && !e.Deleted);

            if (equipment == null)
                throw new NotFoundException($"Equipment with ID '{equipmentId}' not found");

            equipment.OperatingHours = operatingHours;
            _context.Equipment.Update(equipment);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated equipment '{equipment.Name}' operating hours to {operatingHours}");

            return await GetEquipmentByIdAsync(equipmentId);
        }

        #endregion

        #region Equipment Maintenance Management

        /// <summary>
        /// Gets all maintenance records for specific equipment
        /// </summary>
        public async Task<List<EquipmentMaintenanceListViewModel>> GetEquipmentMaintenanceAsync(string equipmentId)
        {
            if (string.IsNullOrEmpty(equipmentId))
                throw new ArgumentException("Equipment ID cannot be empty", nameof(equipmentId));

            var maintenance = await _context.EquipmentMaintenance
                .Include(m => m.Equipment)
                .Where(m => m.EquipmentId == equipmentId && m.IsActive && !m.Deleted)
                .OrderByDescending(m => m.MaintenanceDate)
                .ToListAsync();

            return maintenance.Select(MapToMaintenanceListViewModel).ToList();
        }

        /// <summary>
        /// Gets maintenance record by ID
        /// </summary>
        public async Task<EquipmentMaintenanceViewModel> GetMaintenanceByIdAsync(string maintenanceId)
        {
            if (string.IsNullOrEmpty(maintenanceId))
                throw new ArgumentException("Maintenance ID cannot be empty", nameof(maintenanceId));

            var maintenance = await _context.EquipmentMaintenance
                .Include(m => m.Equipment)
                .FirstOrDefaultAsync(m => m.Id == maintenanceId && m.IsActive && !m.Deleted);

            if (maintenance == null)
                throw new NotFoundException($"Maintenance record with ID '{maintenanceId}' not found");

            return MapToMaintenanceViewModel(maintenance);
        }

        /// <summary>
        /// Creates new maintenance record
        /// </summary>
        public async Task<EquipmentMaintenanceViewModel> CreateMaintenanceAsync(CreateEquipmentMaintenanceViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Verify equipment exists
            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.Id == model.EquipmentId && e.IsActive && !e.Deleted);

            if (equipment == null)
                throw new NotFoundException($"Equipment with ID '{model.EquipmentId}' not found");

            var maintenance = MapFromCreateMaintenanceViewModel(model);

            await _context.EquipmentMaintenance.AddAsync(maintenance);

            // Update equipment's maintenance dates if maintenance is completed
            if (model.IsCompleted)
            {
                equipment.LastMaintenanceDate = model.MaintenanceDate;
                equipment.NextMaintenanceDate = model.NextScheduledMaintenance;
                equipment.OperatingHours = model.OperatingHoursAtMaintenance ?? equipment.OperatingHours;
                _context.Equipment.Update(equipment);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created maintenance record for equipment '{equipment.Name}' on {model.MaintenanceDate:yyyy-MM-dd}");

            return await GetMaintenanceByIdAsync(maintenance.Id);
        }

        /// <summary>
        /// Updates maintenance record
        /// </summary>
        public async Task<EquipmentMaintenanceViewModel> UpdateMaintenanceAsync(EquipmentMaintenanceViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var maintenance = await _context.EquipmentMaintenance
                .Include(m => m.Equipment)
                .FirstOrDefaultAsync(m => m.Id == model.Id && m.IsActive && !m.Deleted);

            if (maintenance == null)
                throw new NotFoundException($"Maintenance record with ID '{model.Id}' not found");

            var wasCompleted = maintenance.IsCompleted;
            
            MapFromMaintenanceViewModelToEntity(model, maintenance);
            
            _context.EquipmentMaintenance.Update(maintenance);

            // Update equipment's maintenance dates if maintenance status changed to completed
            if (model.IsCompleted && !wasCompleted && maintenance.Equipment != null)
            {
                maintenance.Equipment.LastMaintenanceDate = model.MaintenanceDate;
                maintenance.Equipment.NextMaintenanceDate = model.NextScheduledMaintenance;
                maintenance.Equipment.OperatingHours = model.OperatingHoursAtMaintenance ?? maintenance.Equipment.OperatingHours;
                _context.Equipment.Update(maintenance.Equipment);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Updated maintenance record for equipment '{maintenance.Equipment?.Name}' on {model.MaintenanceDate:yyyy-MM-dd}");

            return await GetMaintenanceByIdAsync(maintenance.Id);
        }

        /// <summary>
        /// Deletes maintenance record (soft delete)
        /// </summary>
        public async Task DeleteMaintenanceAsync(string maintenanceId)
        {
            if (string.IsNullOrEmpty(maintenanceId))
                throw new ArgumentException("Maintenance ID cannot be empty", nameof(maintenanceId));

            var maintenance = await _context.EquipmentMaintenance
                .Include(m => m.Equipment)
                .FirstOrDefaultAsync(m => m.Id == maintenanceId && m.IsActive && !m.Deleted);

            if (maintenance == null)
                throw new NotFoundException($"Maintenance record with ID '{maintenanceId}' not found");

            maintenance.IsActive = false;
            maintenance.Deleted = true;

            _context.EquipmentMaintenance.Update(maintenance);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted maintenance record for equipment '{maintenance.Equipment?.Name}'");
        }

        #endregion

        #region Equipment Reporting & Analytics

        /// <summary>
        /// Gets equipment due for maintenance
        /// </summary>
        public async Task<List<EquipmentListViewModel>> GetEquipmentDueForMaintenanceAsync(DateTime dueDate)
        {
            var equipment = await _context.Equipment
                .Include(e => e.Farm)
                .Where(e => e.IsActive && !e.Deleted &&
                           e.NextMaintenanceDate.HasValue &&
                           e.NextMaintenanceDate.Value <= dueDate)
                .ToListAsync();

            return equipment.Select(MapToListViewModel).ToList();
        }

        /// <summary>
        /// Gets equipment by status
        /// </summary>
        public async Task<List<EquipmentListViewModel>> GetEquipmentByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Status cannot be empty", nameof(status));

            if (!Enum.TryParse<EquipmentStatus>(status, out var equipmentStatus))
                throw new ArgumentException($"Invalid equipment status: {status}");

            var equipment = await _context.Equipment
                .Include(e => e.Farm)
                .Where(e => e.IsActive && !e.Deleted && e.Status == equipmentStatus)
                .ToListAsync();

            return equipment.Select(MapToListViewModel).ToList();
        }

        /// <summary>
        /// Gets equipment by condition
        /// </summary>
        public async Task<List<EquipmentListViewModel>> GetEquipmentByConditionAsync(string condition)
        {
            if (string.IsNullOrEmpty(condition))
                throw new ArgumentException("Condition cannot be empty", nameof(condition));

            if (!Enum.TryParse<EquipmentCondition>(condition, out var equipmentCondition))
                throw new ArgumentException($"Invalid equipment condition: {condition}");

            var equipment = await _context.Equipment
                .Include(e => e.Farm)
                .Where(e => e.IsActive && !e.Deleted && e.Condition == equipmentCondition)
                .ToListAsync();

            return equipment.Select(MapToListViewModel).ToList();
        }

        /// <summary>
        /// Gets equipment by type
        /// </summary>
        public async Task<List<EquipmentListViewModel>> GetEquipmentByTypeAsync(string equipmentType)
        {
            if (string.IsNullOrEmpty(equipmentType))
                throw new ArgumentException("Equipment type cannot be empty", nameof(equipmentType));

            if (!Enum.TryParse<EquipmentType>(equipmentType, out var type))
                throw new ArgumentException($"Invalid equipment type: {equipmentType}");

            var equipment = await _context.Equipment
                .Include(e => e.Farm)
                .Where(e => e.IsActive && !e.Deleted && e.Type == type)
                .ToListAsync();

            return equipment.Select(MapToListViewModel).ToList();
        }

        #endregion

        #region Maintenance Analytics

        /// <summary>
        /// Gets total maintenance cost for specific equipment
        /// </summary>
        public async Task<decimal> GetTotalMaintenanceCostAsync(string equipmentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (string.IsNullOrEmpty(equipmentId))
                throw new ArgumentException("Equipment ID cannot be empty", nameof(equipmentId));

            var query = _context.EquipmentMaintenance
                .Where(m => m.EquipmentId == equipmentId && m.IsActive && !m.Deleted && m.Cost.HasValue);

            if (fromDate.HasValue)
                query = query.Where(m => m.MaintenanceDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(m => m.MaintenanceDate <= toDate.Value);

            return await query.SumAsync(m => m.Cost ?? 0);
        }

        /// <summary>
        /// Gets total maintenance cost for all equipment on a farm
        /// </summary>
        public async Task<decimal> GetTotalMaintenanceCostByFarmAsync(string farmId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (string.IsNullOrEmpty(farmId))
                throw new ArgumentException("Farm ID cannot be empty", nameof(farmId));

            var query = _context.EquipmentMaintenance
                .Include(m => m.Equipment)
                .Where(m => m.Equipment.FarmId == farmId && m.IsActive && !m.Deleted && m.Cost.HasValue);

            if (fromDate.HasValue)
                query = query.Where(m => m.MaintenanceDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(m => m.MaintenanceDate <= toDate.Value);

            return await query.SumAsync(m => m.Cost ?? 0);
        }

        /// <summary>
        /// Gets maintenance history for specific equipment
        /// </summary>
        public async Task<List<EquipmentMaintenanceListViewModel>> GetMaintenanceHistoryAsync(string equipmentId)
        {
            return await GetEquipmentMaintenanceAsync(equipmentId);
        }

        /// <summary>
        /// Gets upcoming maintenance across all equipment
        /// </summary>
        public async Task<List<EquipmentMaintenanceListViewModel>> GetUpcomingMaintenanceAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var from = fromDate ?? DateTime.Now;
            var to = toDate ?? DateTime.Now.AddMonths(3);

            var upcomingMaintenance = await _context.Equipment
                .Include(e => e.Farm)
                .Where(e => e.IsActive && !e.Deleted &&
                           e.NextMaintenanceDate.HasValue &&
                           e.NextMaintenanceDate.Value >= from &&
                           e.NextMaintenanceDate.Value <= to)
                .Select(e => new EquipmentMaintenanceListViewModel
                {
                    Id = "", // This is upcoming, not actual maintenance
                    EquipmentName = e.Name,
                    MaintenanceDate = e.NextMaintenanceDate.Value,
                    MaintenanceType = "Scheduled Maintenance",
                    Description = "Scheduled maintenance due",
                    IsCompleted = false
                })
                .ToListAsync();

            return upcomingMaintenance;
        }

        #endregion

        #region Mapping Helper Methods

        private EquipmentListViewModel MapToListViewModel(Equipment equipment)
        {
            return new EquipmentListViewModel
            {
                Id = equipment.Id,
                Name = equipment.Name,
                FarmName = equipment.Farm?.Name ?? "",
                Type = equipment.Type,
                Brand = equipment.Brand,
                Model = equipment.Model,
                Status = equipment.Status,
                Condition = equipment.Condition,
                OperatingHours = equipment.OperatingHours,
                LastMaintenanceDate = equipment.LastMaintenanceDate,
                NextMaintenanceDate = equipment.NextMaintenanceDate
            };
        }

        private EquipmentViewModel MapToViewModel(Equipment equipment)
        {
            return new EquipmentViewModel
            {
                Id = equipment.Id,
                Name = equipment.Name,
                FarmId = equipment.FarmId,
                FarmName = equipment.Farm?.Name,
                Type = equipment.Type,
                Brand = equipment.Brand,
                Model = equipment.Model,
                SerialNumber = equipment.SerialNumber,
                PurchaseDate = equipment.PurchaseDate,
                PurchasePrice = equipment.PurchasePrice,
                WarrantyExpiry = equipment.WarrantyExpiry,
                Status = equipment.Status,
                Condition = equipment.Condition,
                Description = equipment.Description,
                Specifications = equipment.Specifications,
                OperatingHours = equipment.OperatingHours,
                LastMaintenanceDate = equipment.LastMaintenanceDate,
                NextMaintenanceDate = equipment.NextMaintenanceDate,
                MaintenanceNotes = equipment.MaintenanceNotes,
                Location = equipment.Location,
                Notes = equipment.Notes,
                MaintenanceRecords = equipment.MaintenanceRecords?.Select(MapToMaintenanceViewModel).ToList() ?? new List<EquipmentMaintenanceViewModel>()
            };
        }

        private Equipment MapFromCreateViewModel(CreateEquipmentViewModel model)
        {
            return new Equipment
            {
                Name = model.Name,
                FarmId = model.FarmId,
                Type = model.Type,
                Brand = model.Brand,
                Model = model.Model,
                SerialNumber = model.SerialNumber,
                PurchaseDate = model.PurchaseDate,
                PurchasePrice = model.PurchasePrice,
                WarrantyExpiry = model.WarrantyExpiry,
                Status = model.Status,
                Condition = model.Condition,
                Description = model.Description,
                Specifications = model.Specifications,
                OperatingHours = model.OperatingHours,
                LastMaintenanceDate = model.LastMaintenanceDate,
                NextMaintenanceDate = model.NextMaintenanceDate,
                MaintenanceNotes = model.MaintenanceNotes,
                Location = model.Location,
                Notes = model.Notes
            };
        }

        private void MapFromViewModelToEntity(EquipmentViewModel model, Equipment equipment)
        {
            equipment.Name = model.Name;
            equipment.FarmId = model.FarmId;
            equipment.Type = model.Type;
            equipment.Brand = model.Brand;
            equipment.Model = model.Model;
            equipment.SerialNumber = model.SerialNumber;
            equipment.PurchaseDate = model.PurchaseDate;
            equipment.PurchasePrice = model.PurchasePrice;
            equipment.WarrantyExpiry = model.WarrantyExpiry;
            equipment.Status = model.Status;
            equipment.Condition = model.Condition;
            equipment.Description = model.Description;
            equipment.Specifications = model.Specifications;
            equipment.OperatingHours = model.OperatingHours;
            equipment.LastMaintenanceDate = model.LastMaintenanceDate;
            equipment.NextMaintenanceDate = model.NextMaintenanceDate;
            equipment.MaintenanceNotes = model.MaintenanceNotes;
            equipment.Location = model.Location;
            equipment.Notes = model.Notes;
        }

        private EquipmentMaintenanceListViewModel MapToMaintenanceListViewModel(EquipmentMaintenance maintenance)
        {
            return new EquipmentMaintenanceListViewModel
            {
                Id = maintenance.Id,
                EquipmentName = maintenance.Equipment?.Name ?? "",
                MaintenanceDate = maintenance.MaintenanceDate,
                MaintenanceType = maintenance.MaintenanceType,
                Description = maintenance.Description,
                Cost = maintenance.Cost,
                ServiceProvider = maintenance.ServiceProvider,
                IsCompleted = maintenance.IsCompleted,
                NextScheduledMaintenance = maintenance.NextScheduledMaintenance
            };
        }

        private EquipmentMaintenanceViewModel MapToMaintenanceViewModel(EquipmentMaintenance maintenance)
        {
            return new EquipmentMaintenanceViewModel
            {
                Id = maintenance.Id,
                EquipmentId = maintenance.EquipmentId,
                EquipmentName = maintenance.Equipment?.Name,
                MaintenanceDate = maintenance.MaintenanceDate,
                MaintenanceType = maintenance.MaintenanceType,
                Description = maintenance.Description,
                PartsReplaced = maintenance.PartsReplaced,
                Cost = maintenance.Cost,
                ServiceProvider = maintenance.ServiceProvider,
                TechnicianName = maintenance.TechnicianName,
                OperatingHoursAtMaintenance = maintenance.OperatingHoursAtMaintenance,
                NextScheduledMaintenance = maintenance.NextScheduledMaintenance,
                Notes = maintenance.Notes,
                IsCompleted = maintenance.IsCompleted
            };
        }

        private EquipmentMaintenance MapFromCreateMaintenanceViewModel(CreateEquipmentMaintenanceViewModel model)
        {
            return new EquipmentMaintenance
            {
                EquipmentId = model.EquipmentId,
                MaintenanceDate = model.MaintenanceDate,
                MaintenanceType = model.MaintenanceType,
                Description = model.Description,
                PartsReplaced = model.PartsReplaced,
                Cost = model.Cost,
                ServiceProvider = model.ServiceProvider,
                TechnicianName = model.TechnicianName,
                OperatingHoursAtMaintenance = model.OperatingHoursAtMaintenance,
                NextScheduledMaintenance = model.NextScheduledMaintenance,
                Notes = model.Notes,
                IsCompleted = model.IsCompleted
            };
        }

        private void MapFromMaintenanceViewModelToEntity(EquipmentMaintenanceViewModel model, EquipmentMaintenance maintenance)
        {
            maintenance.EquipmentId = model.EquipmentId;
            maintenance.MaintenanceDate = model.MaintenanceDate;
            maintenance.MaintenanceType = model.MaintenanceType;
            maintenance.Description = model.Description;
            maintenance.PartsReplaced = model.PartsReplaced;
            maintenance.Cost = model.Cost;
            maintenance.ServiceProvider = model.ServiceProvider;
            maintenance.TechnicianName = model.TechnicianName;
            maintenance.OperatingHoursAtMaintenance = model.OperatingHoursAtMaintenance;
            maintenance.NextScheduledMaintenance = model.NextScheduledMaintenance;
            maintenance.Notes = model.Notes;
            maintenance.IsCompleted = model.IsCompleted;
        }

        #endregion
    }
}