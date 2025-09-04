using Hurudza.Data.UI.Models.ViewModels.Core;

namespace Hurudza.Data.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing equipment and equipment maintenance operations
    /// </summary>
    public interface IEquipmentService
    {
        // Equipment Management
        Task<List<EquipmentListViewModel>> GetAllEquipmentAsync();
        Task<List<EquipmentListViewModel>> GetEquipmentByFarmAsync(string farmId);
        Task<EquipmentViewModel> GetEquipmentByIdAsync(string equipmentId);
        Task<EquipmentViewModel> CreateEquipmentAsync(CreateEquipmentViewModel model);
        Task<EquipmentViewModel> UpdateEquipmentAsync(EquipmentViewModel model);
        Task DeleteEquipmentAsync(string equipmentId);
        
        // Equipment Status & Condition Management
        Task<EquipmentViewModel> UpdateEquipmentStatusAsync(string equipmentId, string status);
        Task<EquipmentViewModel> UpdateEquipmentConditionAsync(string equipmentId, string condition);
        Task<EquipmentViewModel> UpdateOperatingHoursAsync(string equipmentId, int operatingHours);
        
        // Equipment Maintenance Management
        Task<List<EquipmentMaintenanceListViewModel>> GetEquipmentMaintenanceAsync(string equipmentId);
        Task<EquipmentMaintenanceViewModel> GetMaintenanceByIdAsync(string maintenanceId);
        Task<EquipmentMaintenanceViewModel> CreateMaintenanceAsync(CreateEquipmentMaintenanceViewModel model);
        Task<EquipmentMaintenanceViewModel> UpdateMaintenanceAsync(EquipmentMaintenanceViewModel model);
        Task DeleteMaintenanceAsync(string maintenanceId);
        
        // Equipment Reporting & Analytics
        Task<List<EquipmentListViewModel>> GetEquipmentDueForMaintenanceAsync(DateTime dueDate);
        Task<List<EquipmentListViewModel>> GetEquipmentByStatusAsync(string status);
        Task<List<EquipmentListViewModel>> GetEquipmentByConditionAsync(string condition);
        Task<List<EquipmentListViewModel>> GetEquipmentByTypeAsync(string equipmentType);
        
        // Maintenance Analytics
        Task<decimal> GetTotalMaintenanceCostAsync(string equipmentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<decimal> GetTotalMaintenanceCostByFarmAsync(string farmId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<EquipmentMaintenanceListViewModel>> GetMaintenanceHistoryAsync(string equipmentId);
        Task<List<EquipmentMaintenanceListViewModel>> GetUpcomingMaintenanceAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}