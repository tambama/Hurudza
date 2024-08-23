using System.Net;
using System.Net.Mime;
using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Hurudza.Apis.Base.Models;
using Hurudza.Common.Sms.Services;
using Hurudza.Common.Utils.Exceptions;
using Hurudza.Common.Utils.Extensions;
using Hurudza.Common.Utils.Helpers;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.UI.Models.Models;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Hurudza.Apis.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    //[Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class UsersController : ControllerBase
    {
        private readonly HurudzaDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfigurationProvider _configuration;
        private readonly ISmsService _smsService;
        private readonly EmailSettings _emailSettings;

        public UsersController(
            HurudzaDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfigurationProvider configuration,
            ISmsService smsService,
            IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _smsService = smsService;
            _emailSettings = emailSettings.Value;
        }

        // GET: api/users
        [HttpGet("", Name = nameof(GetUsers))]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .ProjectTo<UserViewModel>(_configuration)
                .ToListAsync();

            return Ok(users);
        }

        // GET api/users/5
        [HttpGet("{id}", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _context.Users
                .ProjectTo<UserViewModel>(_configuration)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("{username}", Name = nameof(GetUserByUsername))]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.Trim().ToLower())
                .ConfigureAwait(false);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("{companyId}", Name = nameof(GetCompanyUsers))]
        public async Task<IActionResult> GetCompanyUsers(string companyId)
        {
            var users = await (from p in _context.UserProfiles
                    where p.FarmId == companyId
                    select p)
                .ProjectTo<UserViewModel>(_configuration)
                .Where(u => !string.IsNullOrEmpty(u.Email))
                .ToListAsync()
                .ConfigureAwait(false);

            return Ok(users);
        }

        // POST api/users
        [HttpPost("", Name = nameof(CreateUser))]
        public async Task<IActionResult> CreateUser([FromBody] UserViewModel vm)
        {
            var user = await _userManager.FindByNameAsync(vm.Email);

            var hasAccount = user != null;

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = vm.Email,
                    Firstname = vm.Firstname,
                    Surname = vm.Surname,
                    Email = vm.Email,
                    PhoneNumber = vm.PhoneNumber.GetMsisdn("0"),
                };

                var result = await _userManager.CreateAsync(user, vm.Password);

                if (!result.Succeeded) throw new CustomException($"Failed to create user {user.UserName}");
            }

            var listProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(p =>
                    p.UserId == user.Id && p.FarmId == vm.FarmId);

            if (listProfile != null)
            {
                listProfile.IsActive = true;
                listProfile.Deleted = false;

                _context.Update(listProfile);
                await _context.SaveChangesAsync();

                return Ok(new ApiOkResponse(await _context.Users.ProjectTo<UserViewModel>(_configuration)
                    .FirstOrDefaultAsync(u => u.Id == user.Id)));
            }

            var role = await _userManager.AddToRoleAsync(user, vm.Role);

            if (!role.Succeeded)
                throw new CustomException($"Failed to add {user.UserName} to role {vm.Role}");

            var profile =
                await _context.UserProfiles.FirstOrDefaultAsync(p =>
                    p.UserId == user.Id && p.FarmId == vm.FarmId);

            if (profile != null)
            {
                // user is in company but in different list
                // move them to this list
                profile.IsActive = true;
                profile.Deleted = false;

                _context.Update(profile);

                await _context.SaveChangesAsync();
            }
            else
            {
                // user is not in this company
                // create a new profile for them
                profile = new UserProfile
                {
                    FarmId = vm.FarmId,
                    UserId = user.Id,
                    Role = vm.Role
                };

                await _context.AddAsync(profile);
                await _context.SaveChangesAsync();
            }

            var response = await _context.Users.ProjectTo<UserViewModel>(_configuration)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            return Ok(new ApiOkResponse(response));
        }
        
        // POST api/users
        [HttpPost("", Name = nameof(InsertCompanyUser))]
        public async Task<IActionResult> InsertCompanyUser([FromBody] UserViewModel vm)
        {
            var user = await _userManager.FindByNameAsync(vm.UserName);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = vm.UserName,
                    Firstname = vm.Firstname,
                    Surname = vm.Surname,
                    Email = vm.Email,
                    PhoneNumber = vm.PhoneNumber.GetMsisdn("0"),
                };

                var result = await _userManager.CreateAsync(user, "Password+1");

                if (!result.Succeeded) throw new CustomException($"Failed to create user {user.UserName}");
            }

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p =>
                    p.UserId == user.Id && p.FarmId == vm.FarmId);

            if (profile != null)
            {
                profile.IsActive = true;
                profile.Deleted = false;

                _context.Update(profile);
                await _context.SaveChangesAsync();

                return Ok(new ApiOkResponse(await _context.Users.ProjectTo<UserViewModel>(_configuration)
                    .FirstOrDefaultAsync(u => u.Id == user.Id), "User already exists!"));
            }

            // user is not in this company
            // create a new profile for them
            profile = new UserProfile
            {
                FarmId = vm.FarmId,
                UserId = user.Id,
                Role = vm.Role
            };

            await _context.AddAsync(profile);
            await _context.SaveChangesAsync();

            var response = await _context.Users.ProjectTo<UserViewModel>(_configuration)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            return Ok(new ApiOkResponse(response, "Successfully added new user!"));
        }

        // PUT api/Users/5
        [HttpPut("{id}", Name = nameof(UpdateUser))]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            user.PhoneNumber = vm.PhoneNumber;
            user.Email = vm.Email;
            user.Firstname = vm.Firstname;
            user.Surname = vm.Surname;
            user.UserName = vm.Email;

            await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(vm.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

                await _userManager.ResetPasswordAsync(user, token, vm.Password);
            }

            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles);

            await _userManager.AddToRoleAsync(user, vm.Role);

            return Ok(new ApiOkResponse(user, "User updated successfully"));
        }
        
        // PUT api/Users/5
        [HttpPut("{id}", Name = nameof(UpdateCompanyUser))]
        public async Task<IActionResult> UpdateCompanyUser(string id, [FromBody] UserViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            user.PhoneNumber = vm.PhoneNumber;
            user.Email = vm.Email;
            user.Firstname = vm.Firstname;
            user.Surname = vm.Surname;
            user.UserName = vm.UserName;

            await _userManager.UpdateAsync(user);

            if (vm.IsAdmin)
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRoleAsync(user, vm.Role);
            }
            else
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.Id == vm.ProfileId)
                    .ConfigureAwait(false);

                profile.Role = vm.Role;

                _context.Update(profile);
            }
            
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Ok(new ApiOkResponse(user, "Successfully updated user details!"));
        }

        // PUT api/Users/5
        [HttpPut("{id}", Name = nameof(ChangeUsername))]
        public async Task<IActionResult> ChangeUsername(string id, [FromBody] UpdateUsernameViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            var exists = await _userManager.FindByNameAsync(vm.Username);

            if (exists != null) throw new CustomException("Username already taken.");

            user.UserName = vm.Username;

            await _userManager.UpdateAsync(user);

            return Ok(user);
        }

        // PUT api/Users/5
        [HttpPut("{id}", Name = nameof(ChangeRole))]
        public async Task<IActionResult> ChangeRole(string id, [FromBody] UpdateUserRoleViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            await _userManager.RemoveFromRoleAsync(user, vm.FromRole);
            await _userManager.AddToRoleAsync(user, vm.ToRole);

            var userProfile =
                await _context.UserProfiles.FirstOrDefaultAsync(p =>
                    p.UserId == user.Id && p.FarmId == vm.FarmId);

            userProfile.Role = vm.ToRole;

            _context.Entry(userProfile).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPut("{id}", Name = nameof(AddToRole))]
        public async Task<IActionResult> AddToRole(string id, [FromBody] UpdateUserRoleViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == id.Trim().ToLower())
                .ConfigureAwait(false);

            if (user == null) return NotFound();

            await _userManager.AddToRoleAsync(user, vm.ToRole);

            return Ok();
        }

        // GET api/users/5
        [AllowAnonymous]
        [HttpGet("{username}", Name = nameof(ForgotPassword))]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) throw new NotFoundException($"User {username} was not found");

            user.TokenValidity = DateTime.Now.AddMinutes(5);
            user.RegistrationToken = Codes.GetRegistrationToken();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) throw new CustomException($"Failed to send reset link. Please try again");

            if (username.IsPhone())
                await _smsService.Send(user.PhoneNumber.GetMsisdn("263"),
                    $"Your password reset code is {user.RegistrationToken}");

            return Ok(new ApiOkResponse(new ResetPasswordViewModel
            {
                Code = user.RegistrationToken,
                Username = user.UserName,
                TokenValidity = user.TokenValidity,
                Token = token
            }, "Succesfully sent password reset code"));
        }

        // POST api/users
        [AllowAnonymous]
        [HttpPost("", Name = nameof(ResetPassword))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel vm)
        {
            var user = await _userManager.FindByNameAsync(vm.Username);

            if (user == null) throw new NotFoundException($"User {vm.Username} was not found");

            var result = await _userManager.ResetPasswordAsync(user, vm.Token, vm.NewPassword);

            if (!result.Succeeded) throw new Exception($"Unable to reset password. Please retry.");

            return Ok(new ApiOkResponse("Password was reset successfully."));
        }

        // POST api/users
        [AllowAnonymous]
        [HttpPost("", Name = nameof(CreatePassword))]
        public async Task<IActionResult> CreatePassword([FromBody] CreatePasswordViewModel vm)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RegistrationToken == vm.RegistrationToken)
                .ConfigureAwait(false);

            if (user == null) throw new NotFoundException($"User was not found");

            var result = await _userManager.AddPasswordAsync(user, vm.Password).ConfigureAwait(false);

            if (!result.Succeeded) throw new Exception($"Unable to reset password. Please retry.");

            return Ok(new ApiOkResponse(vm, "Password was reset successfully."));
        }

        // DELETE api/users/5
        [HttpDelete("{id}", Name = nameof(DeleteUser))]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            user.IsDeleted = true;
            user.IsActive = false;

            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new ApiOkResponse(user, $"{user.Fullname} was deleted successfully"));
        }

        // DELETE api/users/5
        [HttpDelete("{id}", Name = nameof(Deactivate))]
        public async Task<IActionResult> Deactivate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            user.IsActive = false;
            user.IsDeleted = true;

            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // POST api/users
        [HttpPost("", Name = nameof(DeleteFarmUser))]
        public async Task<IActionResult> DeleteFarmUser([FromBody] UserViewModel vm)
        {
            var user = await _userManager.FindByNameAsync(vm.UserName);

            if (user == null)
            {
                return Ok(new Base.Models.ApiResponse((int) HttpStatusCode.NotFound, $"User {user.UserName} was not found"));
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.Id == vm.ProfileId);

            if (profile == null) return Ok(new ApiOkResponse(null, "Successfully added new user!"));
            
            profile.IsActive = false;
            profile.Deleted = true;

            _context.Update(profile);
            await _context.SaveChangesAsync();

            return Ok(new ApiOkResponse(await _context.Users.ProjectTo<UserViewModel>(_configuration)
                    .FirstOrDefaultAsync(u => u.Id == user.Id), 
                $"User {vm.UserName} deactivated successfully!"));

        }
    }
}