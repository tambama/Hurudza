using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Asp.Versioning;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Mapping;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Serilog;

namespace Hurudza.Apis.Core.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class TillageReportsController : Controller
    {
        private readonly HurudzaDbContext _context;
        
        public TillageReportsController(
            HurudzaDbContext context)
        {
            _context = context;
        }
        
        [HttpPost(Name = nameof(GetTillageReport))]
        public async Task<IActionResult> GetTillageReport([FromBody] TillageReportFilterModel filter)
        {
            try
            {
                var reports = await BuildAndExecuteReportQuery(filter);
                
                // Add service type breakdown for each report
                foreach (var report in reports)
                {
                    report.ServiceTypeBreakdown = await GetServiceTypeBreakdown(report.FarmId);
                }
                
                var summary = new TillageReportSummary
                {
                    Reports = reports,
                    Totals = CalculateTotals(reports),
                    ProvinceBreakdown = await GetProvinceBreakdown(filter),
                    MonthlyTrends = await GetMonthlyTrends(filter),
                    GeneratedDate = DateTime.UtcNow,
                    GeneratedBy = User.Identity.Name
                };
                
                return Ok(new ApiOkResponse(summary, "Report generated successfully"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating tillage report");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse((int)HttpStatusCode.InternalServerError,
                        $"Error generating report: {ex.Message}"));
            }
        }
        
        [HttpPost(Name = nameof(ExportTillageReport))]
        public async Task<IActionResult> ExportTillageReport([FromBody] TillageReportFilterModel filter)
        {
            try
            {
                var reports = await BuildAndExecuteReportQuery(filter);
                
                // Add service type breakdown for each report
                foreach (var report in reports)
                {
                    report.ServiceTypeBreakdown = await GetServiceTypeBreakdown(report.FarmId);
                }
                
                using var package = new ExcelPackage();
                
                // Create main report worksheet
                var worksheet = package.Workbook.Worksheets.Add("Tillage Report");
                
                // Add header
                AddReportHeader(worksheet, filter);
                
                // Add column headers
                var currentRow = AddColumnHeaders(worksheet, 5);
                
                // Add data rows
                currentRow = AddDataRows(worksheet, reports, currentRow + 1);
                
                // Add summary section
                AddSummarySection(worksheet, reports, currentRow + 2);
                
                // Create additional worksheets
                await AddProvinceBreakdownSheet(package, filter);
                await AddServiceTypeAnalysisSheet(package, filter);
                await AddMonthlyTrendsSheet(package, filter);
                
                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                
                // Generate file
                var fileName = $"TillageReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var fileContent = package.GetAsByteArray();
                
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error exporting tillage report");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse((int)HttpStatusCode.InternalServerError,
                        $"Error exporting report: {ex.Message}"));
            }
        }
        
        private async Task<List<TillageReportViewModel>> BuildAndExecuteReportQuery(TillageReportFilterModel filter)
        {
            var servicesQuery = _context.TillageServices
                .Include(ts => ts.TillageProgram)
                    .ThenInclude(tp => tp.Farm)
                        .ThenInclude(f => f.Province)
                .Include(ts => ts.TillageProgram)
                    .ThenInclude(tp => tp.Farm)
                        .ThenInclude(f => f.District)
                .Include(ts => ts.TillageProgram)
                    .ThenInclude(tp => tp.Farm)
                        .ThenInclude(f => f.LocalAuthority)
                .Include(ts => ts.TillageProgram)
                    .ThenInclude(tp => tp.Farm)
                        .ThenInclude(f => f.Ward)
                .Include(ts => ts.RecipientFarm)
                .Where(ts => !ts.Deleted && ts.TillageProgram != null && ts.TillageProgram.IsActive);
            
            // Apply filters
            if (!string.IsNullOrEmpty(filter.ProvinceId))
                servicesQuery = servicesQuery.Where(ts => ts.TillageProgram.Farm.ProvinceId == filter.ProvinceId);
                
            if (!string.IsNullOrEmpty(filter.DistrictId))
                servicesQuery = servicesQuery.Where(ts => ts.TillageProgram.Farm.DistrictId == filter.DistrictId);
                
            if (!string.IsNullOrEmpty(filter.LocalAuthorityId))
                servicesQuery = servicesQuery.Where(ts => ts.TillageProgram.Farm.LocalAuthorityId == filter.LocalAuthorityId);
                
            if (filter.Region.HasValue)
                servicesQuery = servicesQuery.Where(ts => ts.TillageProgram.Farm.Region == filter.Region.Value);
                
            if (filter.Conference.HasValue)
                servicesQuery = servicesQuery.Where(ts => ts.TillageProgram.Farm.Conference == filter.Conference.Value);
                
            if (!string.IsNullOrEmpty(filter.TillageProgramId))
                servicesQuery = servicesQuery.Where(ts => ts.TillageProgramId == filter.TillageProgramId);
                
            if (filter.TillageType.HasValue)
                servicesQuery = servicesQuery.Where(ts => ts.TillageType == filter.TillageType.Value);
                
            if (filter.StartDate.HasValue)
                servicesQuery = servicesQuery.Where(ts => ts.ServiceDate >= filter.StartDate.Value);
                
            if (filter.EndDate.HasValue)
                servicesQuery = servicesQuery.Where(ts => ts.ServiceDate <= filter.EndDate.Value);
                
            if (filter.IsCompleted.HasValue)
                servicesQuery = servicesQuery.Where(ts => ts.IsCompleted == filter.IsCompleted.Value);
            
            // Execute query and get data
            var services = await servicesQuery.ToListAsync();
            
            // Group by farm in memory
            var groupedData = services
                .GroupBy(ts => ts.TillageProgram.FarmId)
                .Select(g =>
                {
                    var firstService = g.First();
                    var farm = firstService.TillageProgram.Farm;
                    
                    return new TillageReportViewModel
                    {
                        FarmId = farm.Id,
                        FarmName = farm.Name,
                        Province = farm.Province?.Name ?? "",
                        District = farm.District?.Name ?? "",
                        LocalAuthority = farm.LocalAuthority?.Name ?? "",
                        Ward = farm.Ward?.Name ?? "",
                        Region = farm.Region,
                        Conference = farm.Conference,
                        TargetedHectarage = g.Select(ts => ts.TillageProgram).Distinct().Sum(tp => tp.TotalHectares),
                        CompletedHectarage = g.Where(ts => ts.IsCompleted).Sum(ts => ts.Hectares),
                        NumberOfFarmsTargeted = g.Select(ts => ts.RecipientFarmId).Distinct().Count(),
                        NumberOfFarmsCompleted = g.Where(ts => ts.IsCompleted)
                            .Select(ts => ts.RecipientFarmId).Distinct().Count(),
                        ExpectedIncome = g.Sum(ts => ts.ServiceCost ?? 0),
                        ActualIncome = g.Where(ts => ts.IsCompleted).Sum(ts => ts.ServiceCost ?? 0),
                        IncomeFromCompletedServices = g.Where(ts => ts.IsCompleted).Sum(ts => ts.ServiceCost ?? 0),
                        TotalServices = g.Count(),
                        CompletedServices = g.Count(ts => ts.IsCompleted),
                        PendingServices = g.Count(ts => !ts.IsCompleted),
                        EarliestServiceDate = g.Any() ? g.Min(ts => ts.ServiceDate) : (DateTime?)null,
                        LatestServiceDate = g.Any() ? g.Max(ts => ts.ServiceDate) : (DateTime?)null,
                        AverageHectaresPerService = g.Any() ? g.Average(ts => ts.Hectares) : 0,
                        AverageCostPerHectare = g.Where(ts => ts.Hectares > 0 && ts.ServiceCost.HasValue).Any()
                            ? g.Where(ts => ts.Hectares > 0 && ts.ServiceCost.HasValue)
                                .Average(ts => (decimal)(ts.ServiceCost.Value / (decimal)ts.Hectares))
                            : 0
                    };
                })
                .ToList();
            
            return groupedData;
        }
        
        private async Task<Dictionary<TillageType, ServiceTypeMetrics>> GetServiceTypeBreakdown(string farmId)
        {
            var breakdown = await _context.TillageServices
                .Where(ts => ts.TillageProgram.FarmId == farmId && !ts.Deleted)
                .GroupBy(ts => ts.TillageType)
                .Select(g => new ServiceTypeMetrics
                {
                    TillageType = g.Key,
                    Count = g.Count(),
                    TotalHectares = g.Sum(ts => ts.Hectares),
                    TotalRevenue = g.Sum(ts => ts.ServiceCost ?? 0)
                })
                .ToDictionaryAsync(stm => stm.TillageType);
            
            // Calculate percentages
            var totalHectares = breakdown.Values.Sum(stm => stm.TotalHectares);
            foreach (var metric in breakdown.Values)
            {
                metric.PercentageOfTotal = totalHectares > 0 
                    ? (metric.TotalHectares / totalHectares) * 100 
                    : 0;
            }
            
            return breakdown;
        }
        
        private void AddReportHeader(ExcelWorksheet worksheet, TillageReportFilterModel filter)
        {
            worksheet.Cells["A1:L1"].Merge = true;
            worksheet.Cells["A1"].Value = "TILLAGE SERVICES REPORT";
            worksheet.Cells["A1"].Style.Font.Size = 16;
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            worksheet.Cells["A2:L2"].Merge = true;
            worksheet.Cells["A2"].Value = $"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm}";
            worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            // Add filter information
            var filterInfo = "Filters: ";
            if (!string.IsNullOrEmpty(filter.ProvinceId)) filterInfo += $"Province: {filter.ProvinceId}, ";
            if (!string.IsNullOrEmpty(filter.DistrictId)) filterInfo += $"District: {filter.DistrictId}, ";
            if (filter.StartDate.HasValue) filterInfo += $"From: {filter.StartDate:yyyy-MM-dd}, ";
            if (filter.EndDate.HasValue) filterInfo += $"To: {filter.EndDate:yyyy-MM-dd}";
            
            worksheet.Cells["A3:L3"].Merge = true;
            worksheet.Cells["A3"].Value = filterInfo.TrimEnd(',', ' ');
            worksheet.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
        
        private int AddColumnHeaders(ExcelWorksheet worksheet, int startRow)
        {
            var headers = new[]
            {
                "Farm Name", "Province", "District", "Conference", "Region",
                "Targeted (Ha)", "Completed (Ha)", "Completion %", "Farms Targeted",
                "Expected Income", "Actual Income", "Services"
            };
            
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[startRow, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
            
            return startRow;
        }
        
        private int AddDataRows(ExcelWorksheet worksheet, List<TillageReportViewModel> reports, int startRow)
        {
            var currentRow = startRow;
            
            foreach (var report in reports)
            {
                worksheet.Cells[currentRow, 1].Value = report.FarmName;
                worksheet.Cells[currentRow, 2].Value = report.Province;
                worksheet.Cells[currentRow, 3].Value = report.District;
                worksheet.Cells[currentRow, 4].Value = report.Conference?.ToString() ?? "";
                worksheet.Cells[currentRow, 5].Value = report.Region?.ToString() ?? "";
                worksheet.Cells[currentRow, 6].Value = report.TargetedHectarage;
                worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 7].Value = report.CompletedHectarage;
                worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 8].Value = report.CompletionPercentage;
                worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "0.0%";
                worksheet.Cells[currentRow, 9].Value = report.NumberOfFarmsTargeted;
                worksheet.Cells[currentRow, 10].Value = report.ExpectedIncome;
                worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[currentRow, 11].Value = report.ActualIncome;
                worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[currentRow, 12].Value = $"{report.CompletedServices}/{report.TotalServices}";
                
                // Add borders
                for (int i = 1; i <= 12; i++)
                {
                    worksheet.Cells[currentRow, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
                
                currentRow++;
            }
            
            return currentRow - 1;
        }
        
        private void AddSummarySection(ExcelWorksheet worksheet, List<TillageReportViewModel> reports, int startRow)
        {
            worksheet.Cells[startRow, 1].Value = "SUMMARY";
            worksheet.Cells[startRow, 1].Style.Font.Bold = true;
            worksheet.Cells[startRow, 1].Style.Font.Size = 14;
            
            var summaryRow = startRow + 1;
            worksheet.Cells[summaryRow, 1].Value = "Total Farms:";
            worksheet.Cells[summaryRow, 2].Value = reports.Count;
            
            summaryRow++;
            worksheet.Cells[summaryRow, 1].Value = "Total Targeted Hectares:";
            worksheet.Cells[summaryRow, 2].Value = reports.Sum(r => r.TargetedHectarage);
            worksheet.Cells[summaryRow, 2].Style.Numberformat.Format = "#,##0.00";
            
            summaryRow++;
            worksheet.Cells[summaryRow, 1].Value = "Total Completed Hectares:";
            worksheet.Cells[summaryRow, 2].Value = reports.Sum(r => r.CompletedHectarage);
            worksheet.Cells[summaryRow, 2].Style.Numberformat.Format = "#,##0.00";
            
            summaryRow++;
            worksheet.Cells[summaryRow, 1].Value = "Overall Completion:";
            var totalTargeted = reports.Sum(r => r.TargetedHectarage);
            var totalCompleted = reports.Sum(r => r.CompletedHectarage);
            var overallCompletion = totalTargeted > 0 ? totalCompleted / totalTargeted : 0;
            worksheet.Cells[summaryRow, 2].Value = overallCompletion;
            worksheet.Cells[summaryRow, 2].Style.Numberformat.Format = "0.0%";
            
            summaryRow++;
            worksheet.Cells[summaryRow, 1].Value = "Total Expected Income:";
            worksheet.Cells[summaryRow, 2].Value = reports.Sum(r => r.ExpectedIncome);
            worksheet.Cells[summaryRow, 2].Style.Numberformat.Format = "$#,##0.00";
            
            summaryRow++;
            worksheet.Cells[summaryRow, 1].Value = "Total Actual Income:";
            worksheet.Cells[summaryRow, 2].Value = reports.Sum(r => r.ActualIncome);
            worksheet.Cells[summaryRow, 2].Style.Numberformat.Format = "$#,##0.00";
        }
        
        private async Task AddProvinceBreakdownSheet(ExcelPackage package, TillageReportFilterModel filter)
        {
            var worksheet = package.Workbook.Worksheets.Add("Province Breakdown");
            var breakdown = await GetProvinceBreakdown(filter);
            
            // Headers
            worksheet.Cells["A1"].Value = "Province";
            worksheet.Cells["B1"].Value = "Farms";
            worksheet.Cells["C1"].Value = "Total Hectares";
            worksheet.Cells["D1"].Value = "Completed Hectares";
            worksheet.Cells["E1"].Value = "Completion %";
            worksheet.Cells["F1"].Value = "Revenue";
            
            // Style headers
            for (int i = 1; i <= 6; i++)
            {
                worksheet.Cells[1, i].Style.Font.Bold = true;
                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }
            
            // Data
            int row = 2;
            foreach (var province in breakdown)
            {
                worksheet.Cells[row, 1].Value = province.ProvinceName;
                worksheet.Cells[row, 2].Value = province.FarmCount;
                worksheet.Cells[row, 3].Value = province.TotalHectares;
                worksheet.Cells[row, 4].Value = province.CompletedHectares;
                worksheet.Cells[row, 5].Value = province.CompletionPercentage / 100;
                worksheet.Cells[row, 5].Style.Numberformat.Format = "0.0%";
                worksheet.Cells[row, 6].Value = province.TotalRevenue;
                worksheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
                row++;
            }
            
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }
        
        private async Task AddServiceTypeAnalysisSheet(ExcelPackage package, TillageReportFilterModel filter)
        {
            var worksheet = package.Workbook.Worksheets.Add("Service Type Analysis");
            
            var serviceTypes = await _context.TillageServices
                .Where(ts => !ts.Deleted && ts.TillageProgram.IsActive)
                .GroupBy(ts => ts.TillageType)
                .Select(g => new
                {
                    TillageType = g.Key,
                    Count = g.Count(),
                    TotalHectares = g.Sum(ts => ts.Hectares),
                    TotalRevenue = g.Sum(ts => ts.ServiceCost ?? 0),
                    AverageHectares = g.Average(ts => ts.Hectares),
                    AverageRevenue = g.Average(ts => ts.ServiceCost ?? 0)
                })
                .ToListAsync();
            
            // Headers
            worksheet.Cells["A1"].Value = "Tillage Type";
            worksheet.Cells["B1"].Value = "Service Count";
            worksheet.Cells["C1"].Value = "Total Hectares";
            worksheet.Cells["D1"].Value = "Total Revenue";
            worksheet.Cells["E1"].Value = "Avg Hectares/Service";
            worksheet.Cells["F1"].Value = "Avg Revenue/Service";
            
            // Style headers
            for (int i = 1; i <= 6; i++)
            {
                worksheet.Cells[1, i].Style.Font.Bold = true;
                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            }
            
            // Data
            int row = 2;
            foreach (var type in serviceTypes)
            {
                worksheet.Cells[row, 1].Value = type.TillageType.ToString();
                worksheet.Cells[row, 2].Value = type.Count;
                worksheet.Cells[row, 3].Value = type.TotalHectares;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[row, 4].Value = type.TotalRevenue;
                worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[row, 5].Value = type.AverageHectares;
                worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[row, 6].Value = type.AverageRevenue;
                worksheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
                row++;
            }
            
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }
        
        private async Task AddMonthlyTrendsSheet(ExcelPackage package, TillageReportFilterModel filter)
        {
            var worksheet = package.Workbook.Worksheets.Add("Monthly Trends");
            var trends = await GetMonthlyTrends(filter);
            
            // Headers
            worksheet.Cells["A1"].Value = "Month";
            worksheet.Cells["B1"].Value = "Hectares Completed";
            worksheet.Cells["C1"].Value = "Revenue";
            worksheet.Cells["D1"].Value = "Services Completed";
            
            // Style headers
            for (int i = 1; i <= 4; i++)
            {
                worksheet.Cells[1, i].Style.Font.Bold = true;
                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            }
            
            // Data
            int row = 2;
            foreach (var trend in trends)
            {
                worksheet.Cells[row, 1].Value = $"{trend.MonthName} {trend.Year}";
                worksheet.Cells[row, 2].Value = trend.HectaresCompleted;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[row, 3].Value = trend.Revenue;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[row, 4].Value = trend.ServicesCompleted;
                row++;
            }
            
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }
        
        private TotalMetrics CalculateTotals(List<TillageReportViewModel> reports)
        {
            return new TotalMetrics
            {
                TotalFarms = reports.Count,
                TotalTargetedHectares = reports.Sum(r => r.TargetedHectarage),
                TotalCompletedHectares = reports.Sum(r => r.CompletedHectarage),
                TotalExpectedIncome = reports.Sum(r => r.ExpectedIncome),
                TotalActualIncome = reports.Sum(r => r.ActualIncome),
                TotalServices = reports.Sum(r => r.TotalServices),
                TotalCompletedServices = reports.Sum(r => r.CompletedServices),
                OverallCompletionPercentage = reports.Sum(r => r.TargetedHectarage) > 0 
                    ? (reports.Sum(r => r.CompletedHectarage) / reports.Sum(r => r.TargetedHectarage)) * 100 
                    : 0
            };
        }
        
        private async Task<List<ProvinceMetrics>> GetProvinceBreakdown(TillageReportFilterModel filter)
        {
            var query = _context.TillageServices
                .Include(ts => ts.TillageProgram)
                    .ThenInclude(tp => tp.Farm)
                        .ThenInclude(f => f.Province)
                .Where(ts => !ts.Deleted && ts.TillageProgram.IsActive);
            
            // Apply same filters as main query
            if (filter.StartDate.HasValue)
                query = query.Where(ts => ts.ServiceDate >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                query = query.Where(ts => ts.ServiceDate <= filter.EndDate.Value);
            
            return await query
                .GroupBy(ts => new 
                { 
                    ts.TillageProgram.Farm.ProvinceId,
                    ProvinceName = ts.TillageProgram.Farm.Province.Name 
                })
                .Select(g => new ProvinceMetrics
                {
                    ProvinceId = g.Key.ProvinceId,
                    ProvinceName = g.Key.ProvinceName,
                    FarmCount = g.Select(ts => ts.TillageProgram.FarmId).Distinct().Count(),
                    TotalHectares = g.Sum(ts => ts.Hectares),
                    CompletedHectares = g.Where(ts => ts.IsCompleted).Sum(ts => ts.Hectares),
                    TotalRevenue = g.Sum(ts => ts.ServiceCost ?? 0),
                    CompletionPercentage = g.Sum(ts => ts.Hectares) > 0 
                        ? (g.Where(ts => ts.IsCompleted).Sum(ts => ts.Hectares) / g.Sum(ts => ts.Hectares)) * 100 
                        : 0
                })
                .OrderByDescending(p => p.TotalHectares)
                .ToListAsync();
        }
        
        private async Task<List<MonthlyMetrics>> GetMonthlyTrends(TillageReportFilterModel filter)
        {
            var query = _context.TillageServices
                .Where(ts => !ts.Deleted && ts.TillageProgram.IsActive && ts.IsCompleted);
            
            // Apply filters
            if (filter.StartDate.HasValue)
                query = query.Where(ts => ts.ServiceDate >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                query = query.Where(ts => ts.ServiceDate <= filter.EndDate.Value);
            
            var trends = await query
                .GroupBy(ts => new { ts.ServiceDate.Year, ts.ServiceDate.Month })
                .Select(g => new MonthlyMetrics
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    HectaresCompleted = g.Sum(ts => ts.Hectares),
                    Revenue = g.Sum(ts => ts.ServiceCost ?? 0),
                    ServicesCompleted = g.Count()
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToListAsync();
            
            // Add month names
            foreach (var trend in trends)
            {
                trend.MonthName = new DateTime(trend.Year, trend.Month, 1).ToString("MMMM");
            }
            
            return trends;
        }
    }
}