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
public class EquipmentController : Controller
{
    private readonly IEquipmentService _equipmentService;
    private readonly UserManager<ApplicationUser> _userManager;

    public EquipmentController(IEquipmentService equipmentService, 
        UserManager<ApplicationUser> userManager)
    {
        _equipmentService = equipmentService;
        _userManager = userManager;
    }

    [HttpGet(Name = nameof(GetEquipment))]
    public async Task<IActionResult> GetEquipment()
    {
        try
        {
            var equipment = await _equipmentService.GetAllEquipmentAsync().ConfigureAwait(false);
            return Ok(new ApiOkResponse(equipment));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading equipment: {ex.Message}"));
        }
    }

    [HttpGet("{farmId}", Name = nameof(GetEquipmentByFarm))]
    public async Task<IActionResult> GetEquipmentByFarm(string farmId)
    {
        try
        {
            var equipment = await _equipmentService.GetEquipmentByFarmAsync(farmId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(equipment));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading equipment: {ex.Message}"));
        }
    }

    [HttpGet("{id}", Name = nameof(GetEquipmentDetails))]
    public async Task<IActionResult> GetEquipmentDetails(string id)
    {
        try
        {
            var equipment = await _equipmentService.GetEquipmentByIdAsync(id).ConfigureAwait(false);
            return Ok(equipment == null
                ? new ApiResponse((int)HttpStatusCode.NotFound, "Equipment not found")
                : new ApiOkResponse(equipment));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading equipment: {ex.Message}"));
        }
    }

    [HttpPost(Name = nameof(CreateEquipment))]
    public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentViewModel model)
    {
        try
        {
            var equipment = await _equipmentService.CreateEquipmentAsync(model).ConfigureAwait(false);
            return Ok(new ApiOkResponse(equipment, "Equipment successfully created"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error creating equipment: {ex.Message}"));
        }
    }

    [HttpPut("{id}", Name = nameof(UpdateEquipment))]
    public async Task<IActionResult> UpdateEquipment(string id, [FromBody] EquipmentViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var equipment = await _equipmentService.UpdateEquipmentAsync(model).ConfigureAwait(false);
            if (equipment == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Equipment not found"));
            }
            return Ok(new ApiOkResponse(equipment, $"{equipment.Name} successfully updated"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error updating equipment: {ex.Message}"));
        }
    }

    [HttpDelete("{id}", Name = nameof(DeleteEquipment))]
    public async Task<IActionResult> DeleteEquipment(string id)
    {
        try
        {
            await _equipmentService.DeleteEquipmentAsync(id).ConfigureAwait(false);
            return Ok(new ApiOkResponse("Equipment was deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error deleting equipment: {ex.Message}"));
        }
    }

    #region Equipment Maintenance

    [HttpGet("{equipmentId}/maintenance", Name = nameof(GetEquipmentMaintenance))]
    public async Task<IActionResult> GetEquipmentMaintenance(string equipmentId)
    {
        try
        {
            var maintenance = await _equipmentService.GetMaintenanceHistoryAsync(equipmentId).ConfigureAwait(false);
            return Ok(new ApiOkResponse(maintenance));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading maintenance history: {ex.Message}"));
        }
    }

    [HttpGet("maintenance/{id}", Name = nameof(GetMaintenanceDetails))]
    public async Task<IActionResult> GetMaintenanceDetails(string id)
    {
        try
        {
            var maintenance = await _equipmentService.GetMaintenanceByIdAsync(id).ConfigureAwait(false);
            return Ok(maintenance == null
                ? new ApiResponse((int)HttpStatusCode.NotFound, "Maintenance record not found")
                : new ApiOkResponse(maintenance));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error loading maintenance record: {ex.Message}"));
        }
    }

    [HttpPost("maintenance", Name = nameof(CreateMaintenance))]
    public async Task<IActionResult> CreateMaintenance([FromBody] CreateEquipmentMaintenanceViewModel model)
    {
        try
        {
            var maintenance = await _equipmentService.CreateMaintenanceAsync(model).ConfigureAwait(false);
            return Ok(new ApiOkResponse(maintenance, "Maintenance record successfully created"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error creating maintenance record: {ex.Message}"));
        }
    }

    [HttpPut("maintenance/{id}", Name = nameof(UpdateMaintenance))]
    public async Task<IActionResult> UpdateMaintenance(string id, [FromBody] EquipmentMaintenanceViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var maintenance = await _equipmentService.UpdateMaintenanceAsync(model).ConfigureAwait(false);
            if (maintenance == null)
            {
                return Ok(new ApiResponse((int)HttpStatusCode.NotFound, "Maintenance record not found"));
            }
            return Ok(new ApiOkResponse(maintenance, "Maintenance record successfully updated"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error updating maintenance record: {ex.Message}"));
        }
    }

    [HttpDelete("maintenance/{id}", Name = nameof(DeleteMaintenance))]
    public async Task<IActionResult> DeleteMaintenance(string id)
    {
        try
        {
            await _equipmentService.DeleteMaintenanceAsync(id).ConfigureAwait(false);
            return Ok(new ApiOkResponse("Maintenance record was deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse(500, $"Error deleting maintenance record: {ex.Message}"));
        }
    }

    #endregion
}