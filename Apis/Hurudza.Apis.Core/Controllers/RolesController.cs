using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers;

[Route("api/[controller]/[action]")]
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class RolesController : ControllerBase
{
    private readonly HurudzaDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfigurationProvider _configuration;

    public RolesController(
        HurudzaDbContext context,
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfigurationProvider configuration)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpGet(Name = nameof(GetRoles))]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _context.Roles
            .ProjectTo<RoleViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        // Load permissions for each role
        foreach (var role in roles)
        {
            var appRole = await _roleManager.FindByIdAsync(role.Id);
            if (appRole != null)
            {
                var claims = await _roleManager.GetClaimsAsync(appRole);
                role.Permissions = claims.Select(c => new ClaimViewModel
                {
                    ClaimType = c.Type,
                    ClaimValue = c.Value
                }).ToList();
            }
        }

        return Ok(roles);
    }

    [HttpGet("{id}", Name = nameof(GetRole))]
    public async Task<IActionResult> GetRole(string id)
    {
        var role = await _context.Roles
            .ProjectTo<RoleViewModel>(_configuration)
            .FirstOrDefaultAsync(r => r.Id == id)
            .ConfigureAwait(false);

        if (role == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Role not found"));

        // Load permissions
        var appRole = await _roleManager.FindByIdAsync(id);
        if (appRole != null)
        {
            var claims = await _roleManager.GetClaimsAsync(appRole);
            role.Permissions = claims.Select(c => new ClaimViewModel
            {
                ClaimType = c.Type,
                ClaimValue = c.Value
            }).ToList();
        }

        return Ok(new ApiOkResponse(role));
    }

    [HttpPost(Name = nameof(CreateRole))]
    public async Task<IActionResult> CreateRole([FromBody] RoleViewModel model)
    {
        if (await _roleManager.RoleExistsAsync(model.Name))
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Role already exists"));

        var newRole = new ApplicationRole
        {
            Name = model.Name,
            Description = model.Description,
            RoleClass = RoleClass.General // Default to General class
        };

        var result = await _roleManager.CreateAsync(newRole);
        if (!result.Succeeded)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Failed to create role"));

        // Add claims/permissions to the role
        if (model.Permissions != null && model.Permissions.Any())
        {
            foreach (var permission in model.Permissions)
            {
                await _roleManager.AddClaimAsync(newRole, new Claim(permission.ClaimType, permission.ClaimValue));
            }
        }

        var createdRole = await _context.Roles
            .ProjectTo<RoleViewModel>(_configuration)
            .FirstOrDefaultAsync(r => r.Id == newRole.Id);

        return Ok(new ApiOkResponse(createdRole, "Role created successfully"));
    }

    [HttpPut("{id}", Name = nameof(UpdateRole))]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleViewModel model)
    {
        if (id != model.Id)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Role ID mismatch"));

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Role not found"));

        // Update basic properties
        role.Name = model.Name;
        role.Description = model.Description;

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Failed to update role"));

        // Update claims/permissions
        // First, remove all existing claims
        var existingClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var claim in existingClaims)
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }

        // Then add the new claims
        if (model.Permissions != null && model.Permissions.Any())
        {
            foreach (var permission in model.Permissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim(permission.ClaimType, permission.ClaimValue));
            }
        }

        var updatedRole = await _context.Roles
            .ProjectTo<RoleViewModel>(_configuration)
            .FirstOrDefaultAsync(r => r.Id == role.Id);

        return Ok(new ApiOkResponse(updatedRole, "Role updated successfully"));
    }

    [HttpDelete("{id}", Name = nameof(DeleteRole))]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "Role not found"));

        // Check if any users are assigned to this role
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        if (usersInRole.Any())
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Cannot delete role as it is assigned to users"));

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Failed to delete role"));

        return Ok(new ApiOkResponse(null, "Role deleted successfully"));
    }

    [HttpGet(Name = nameof(GetRolesByClass))]
    public async Task<IActionResult> GetRolesByClass([FromQuery] RoleClass roleClass)
    {
        var roles = await _context.Roles
            .Where(r => r.RoleClass == roleClass)
            .ProjectTo<RoleViewModel>(_configuration)
            .ToListAsync()
            .ConfigureAwait(false);

        return Ok(roles);
    }
}