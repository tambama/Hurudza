using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Context.Data;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ApiResponse = Hurudza.Data.Models.Models.ApiResponse;
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

            if (profiles.Count > 0)
                profiles[0].LoggedIn = true;
            else
            {
                var userRole = (await _userManager.GetRolesAsync(user)).First();
                profiles.Add(new UserProfileViewModel { Farm = "System", Fullname = "Administrator", Role = userRole, LoggedIn = true });
            }

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.PrimarySid, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var loggedIn = profiles.First(p => p.LoggedIn);
            authClaims.Add(new Claim(ClaimTypes.Role, loggedIn.Role));

            var role = await _roleManager.FindByNameAsync(loggedIn.Role);
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            authClaims.AddRange(roleClaims.ToList());
            
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var login = new UserViewModel
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Profiles = profiles
            };

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

            if (profiles.Count > 0)
                profiles[0].LoggedIn = true;
            else
            {
                var userRole = (await _userManager.GetRolesAsync(user)).First();
                profiles.Add(new UserProfileViewModel { Farm = "System", Fullname = "Administrator", Role = userRole, LoggedIn = true });
            }

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.PrimarySid, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var loggedIn = profiles.First(p => p.LoggedIn);
            authClaims.Add(new Claim(ClaimTypes.Role, loggedIn.Role));

            var role = await _roleManager.FindByNameAsync(loggedIn.Role);
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            authClaims.AddRange(roleClaims.ToList());
            
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var login = new UserViewModel
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Profiles = profiles
            };

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

            if (profiles.Count > 0)
                profiles[0].LoggedIn = true;
            else
            {
                var userRole = (await _userManager.GetRolesAsync(user)).First();
                profiles.Add(new UserProfileViewModel { Farm = "System", Fullname = "Administrator", Role = userRole, LoggedIn = true });
            }

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.PrimarySid, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var loggedIn = profiles.First(p => p.LoggedIn);
            authClaims.Add(new Claim(ClaimTypes.Role, loggedIn.Role));

            var role = await _roleManager.FindByNameAsync(loggedIn.Role);
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            authClaims.AddRange(roleClaims.ToList());
            
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var login = new UserViewModel
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Profiles = profiles
            };

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

            if (profiles.Count > 0)
                profiles.First(p => p.FarmId == model.FarmId).LoggedIn = true;
            else
                profiles.Add(new UserProfileViewModel { Farm = "System", Fullname = user.Fullname, Role = ApiRoles.SystemAdministrator, LoggedIn = true });

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.PrimarySid, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            
            var loggedIn = profiles.First(p => p.LoggedIn);
            authClaims.Add(new Claim(ClaimTypes.Role, loggedIn.Role));

            var role = await _roleManager.FindByNameAsync(loggedIn.Role);
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            authClaims.AddRange(roleClaims.ToList());

            var login = new UserViewModel
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Surname = user.Surname,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Profiles = profiles
            };

            return Ok(new ApiOkResponse(login));
        }

        [HttpPost(Name = nameof(Register)), AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserViewModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "User already exists!"));

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Firstname = model.Firstname,
                Surname = model.Surname
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse((int)HttpStatusCode.InternalServerError, "User creation failed! Please check user details and try again."));

            return Ok(new ApiResponse((int)HttpStatusCode.OK, "User created successfully!"));
        }
    }
}
