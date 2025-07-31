using AutoMapper;
using beaconinteriorsapi.Data;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;

namespace beaconinteriorsapi.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly BeaconInteriorsDBContext _dbContext;
        private readonly IMapper _mapper;


        public UserService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            BeaconInteriorsDBContext dbContext, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) ThrowNotFound($"no user found with id {userId}");
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetUsersByEmailOrNameAsync(string value)
        {
           var users= await _dbContext.Users.Where(u => u.FirstName.Contains(value) || u.LastName.Contains(value)|| u.Email!.Contains(value)).ToListAsync();
            return _mapper.Map<UserDTO>(users);
        }

        public async Task UpdateUserInfoAsync(string userId, string newFirstName, string newLastName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                ThrowNotFound($"User with ID '{userId}' not found");

            user!.FirstName = newFirstName;
            user.LastName = newLastName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                ThrowBadRequest("Failed to update user info");
        }



        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                ThrowNotFound($"User with ID '{userId}' not found");

            var result = await _userManager.DeleteAsync(user!);
            if (!result.Succeeded)
                ThrowBadRequest("Failed to delete user");
        }

        public async Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                ThrowBadRequest($"Role '{roleName}' does not exist");

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            return _mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(usersInRole.ToList());

        }

        public async Task MakeSuperAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");

            var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");
            if (isSuperAdmin) return; // already super admin
            var isAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");
            if (isAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                await _userManager.AddToRoleAsync(user, "SuperAdmin");
            }
            else
            {
                ThrowBadRequest("only admins can be upgraded to superadmin");
            }
        }

        // Force password reset for a user (e.g., for superadmin use)
        //public async Task ForcePasswordResetAsync(string userId, string newPassword)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        ThrowNotFound($"User with ID '{userId}' not found");

        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //    var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        //    if (!result.Succeeded)
        //        ThrowBadRequest("Password reset failed");
        //}
    }
}
