using System.Net.Mime;
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