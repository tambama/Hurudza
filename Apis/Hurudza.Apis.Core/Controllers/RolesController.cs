using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Common.Emails.Helpers;
using Hurudza.Common.Emails.Services;
using Hurudza.Common.Sms.Services;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ApiResponse = Hurudza.Apis.Base.Models.ApiResponse;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class RolesController : ControllerBase
    {
        private readonly HurudzaDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfigurationProvider _configuration;
        private readonly ISmsService _smsService;
        private readonly ISendGridEmailService _sendGridEmailService;
        private readonly ISendGridMessageHelper _sendGridMessageHelper;
        private readonly EmailSettings _emailSettings;

        public RolesController(
            HurudzaDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfigurationProvider configuration,
            ISmsService smsService,
            IOptions<EmailSettings> emailSettings,
            ISendGridEmailService sendGridEmailService,
            ISendGridMessageHelper sendGridMessageHelper)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _smsService = smsService;
            _emailSettings = emailSettings.Value;
            _sendGridEmailService = sendGridEmailService;
            _sendGridMessageHelper = sendGridMessageHelper;
        }

        [HttpGet(Name = nameof(GetRoles))]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .ProjectTo<RoleViewModel>(_configuration)
                .ToListAsync()
                .ConfigureAwait(false);
            
            return Ok(roles);
        }

        [HttpPost(Name = nameof(CreateRole))]
        public async Task<IActionResult> CreateRole([FromBody] RoleViewModel vm)
        {
            if (await _roleManager.RoleExistsAsync(vm.Name))
                return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Role already exists"));

            var newRole = await _roleManager.CreateAsync(new ApplicationRole
            {
                Name = vm.Name,
                Description = vm.Description
            }).ConfigureAwait(false);

            if (!newRole.Succeeded)
                return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Failed to add Role"));

            var role = await _roleManager.FindByNameAsync(vm.Name).ConfigureAwait(false);

            if (role is null)
                return Ok(new ApiResponse((int)HttpStatusCode.BadRequest, "Failed to add Role"));

            if (vm.Permissions != null)
                foreach (var permission in vm.Permissions)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(permission.ClaimType, permission.ClaimValue))
                        .ConfigureAwait(false);
                }

            var result = await _context.Roles.ProjectTo<RoleViewModel>(_configuration)
                .FirstOrDefaultAsync(r => r.Id == role.Id).ConfigureAwait(false);
            
            return Ok(new ApiOkResponse(result));
        }

        [HttpPost(Name = nameof(FilterRoles))]
        public async Task<IActionResult> FilterRoles([FromBody] FilterRolesViewModel vm)
        {
            var roles = await _context.Roles.Where(r =>
                    vm.RoleClasses.Count == 0 ||
                    vm.RoleClasses.Contains(r.RoleClass))
                .ProjectTo<RoleViewModel>(_configuration)
                .ToListAsync()
                .ConfigureAwait(false);

            return Ok(new ApiOkResponse(roles));
        }
    }
}
