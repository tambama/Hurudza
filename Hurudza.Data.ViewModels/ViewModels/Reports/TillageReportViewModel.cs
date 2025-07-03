using System;
using System.Collections.Generic;
using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.ViewModels.Reports
{
    public class TillageReportViewModel
    {
        // Farm Information
        public string FarmId { get; set; }
        public string FarmName { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string LocalAuthority { get; set; }
        public string Ward { get; set; }
        public Region? Region { get; set; }
        public Conference? Conference { get; set; }
        
        // Tillage Metrics
        public float TargetedHectarage { get; set; }
        public float CompletedHectarage { get; set; }
        public float CompletionPercentage => TargetedHectarage > 0 
            ? (CompletedHectarage / TargetedHectarage) * 100 
            : 0;
        
        // Farm Counts
        public int NumberOfFarmsTargeted { get; set; }
        public int NumberOfFarmsCompleted { get; set; }
        
        // Financial Metrics
        public decimal ExpectedIncome { get; set; }
        public decimal ActualIncome { get; set; }
        public decimal IncomeFromCompletedServices { get; set; }
        
        // Service Details
        public int TotalServices { get; set; }
        public int CompletedServices { get; set; }
        public int PendingServices { get; set; }
        
        // Service Type Breakdown
        public Dictionary<TillageType, ServiceTypeMetrics> ServiceTypeBreakdown { get; set; }
        
        // Time-based Metrics
        public DateTime? EarliestServiceDate { get; set; }
        public DateTime? LatestServiceDate { get; set; }
        public float AverageHectaresPerService { get; set; }
        public decimal AverageCostPerHectare { get; set; }
    }
    
    public class ServiceTypeMetrics
    {
        public TillageType TillageType { get; set; }
        public int Count { get; set; }
        public float TotalHectares { get; set; }
        public decimal TotalRevenue { get; set; }
        public float PercentageOfTotal { get; set; }
    }
    
    public class TillageReportFilterModel
    {
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
        public string? LocalAuthorityId { get; set; }
        public string? WardId { get; set; }
        public Region? Region { get; set; }
        public Conference? Conference { get; set; }
        public string? TillageProgramId { get; set; }
        public TillageType? TillageType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsCompleted { get; set; }
        public string? FarmId { get; set; }
        
        // Additional filter options
        public decimal? MinIncome { get; set; }
        public decimal? MaxIncome { get; set; }
        public float? MinHectares { get; set; }
        public float? MaxHectares { get; set; }
    }
    
    public class TillageReportSummary
    {
        public List<TillageReportViewModel> Reports { get; set; }
        public TotalMetrics Totals { get; set; }
        public List<ProvinceMetrics> ProvinceBreakdown { get; set; }
        public List<MonthlyMetrics> MonthlyTrends { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string GeneratedBy { get; set; }
    }
    
    public class TotalMetrics
    {
        public int TotalFarms { get; set; }
        public float TotalTargetedHectares { get; set; }
        public float TotalCompletedHectares { get; set; }
        public float OverallCompletionPercentage { get; set; }
        public decimal TotalExpectedIncome { get; set; }
        public decimal TotalActualIncome { get; set; }
        public int TotalServices { get; set; }
        public int TotalCompletedServices { get; set; }
    }
    
    public class ProvinceMetrics
    {
        public string ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int FarmCount { get; set; }
        public float TotalHectares { get; set; }
        public float CompletedHectares { get; set; }
        public decimal TotalRevenue { get; set; }
        public float CompletionPercentage { get; set; }
    }
    
    public class MonthlyMetrics
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public float HectaresCompleted { get; set; }
        public decimal Revenue { get; set; }
        public int ServicesCompleted { get; set; }
    }
}