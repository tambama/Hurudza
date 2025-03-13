using System.Net.Mime;
using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Hurudza.Data.Context.Context;
using Hurudza.Data.UI.Models.ViewModels.Stats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class StatisticsController : Controller
{
    private readonly HurudzaDbContext _context;
    private readonly IConfigurationProvider _configuration;

    public StatisticsController(HurudzaDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    // GET
    [HttpGet]
    public async Task<IActionResult> GetBaseStatistics()
    {
        var schools = await _context.Farms.ToListAsync().ConfigureAwait(false);
        var stats = new BaseStatisticsViewModel()
        {
            Centers = schools.Count,
            TotalLand = schools.Sum(s => s.Size),
            Arable = schools.Sum(s => s.Arable),
            Tillage = 0
        };
        
        return Ok(stats);
    }

    [HttpGet(Name = "GetBigFiveStatistics")]
    public async Task<IActionResult> GetBigFiveStatistics()
    {
        var schools = await _context.Farms
            .OrderByDescending(s => s.Size)
            .Take(5)
            .ProjectTo<FarmStatisticsViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);
        
        return Ok(schools);
    }
}