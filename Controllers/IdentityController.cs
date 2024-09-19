using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCrawler.Data;
using Microsoft.EntityFrameworkCore;
using WebApiCrawler.ShopModels.SearchModels;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace WebApiCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityContext _context;
        private readonly NLog.ILogger _logger;

        public IdentityController(IdentityContext dbContext, NLog.ILogger logger)
        {
            _context = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Adds the basic "user" role to a specified user.
        /// </summary>
        /// <param name="userId">The ID of the user to whom the role will be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [HttpGet("AddRole")]
        public async Task AddBasicRole(string userId)
        {
            string roleId = _context.Roles.FirstOrDefault(u => u.Name == "user").Id;
            _context.UserRoles.Add(new IdentityUserRole<string> { UserId = userId, RoleId = roleId });
            await _context.SaveChangesAsync();
        }
        /*
        /// <summary>
        /// Retrieves a paginated list of users, optionally filtered by a search term.
        /// </summary>
        /// <param name="filter">An optional filter string to search for users by their full name.</param>
        /// <param name="page">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of users per page (default is 10).</param>
        /// <returns>A task that represents the asynchronous operation, containing an IActionResult with the list of users.</returns>

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers(string? filter, int page = 1, int pageSize = 10)
        {
            var usersWithRoles = await _context.Users
                .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { User = u, UserRole = ur })
                .Join(_context.Roles, ur => ur.UserRole.RoleId, r => r.Id, (ur, r) => new { User = ur.User, RoleName = r.Name })
                .Select(ur => new IdentityModel
                {
                    id = ur.User.Id,
                    fullname = ur.User.FullName,
                    email = ur.User.Email,
                    username = ur.User.UserName,
                    avatar = "",
                    active = ur.User.EmailConfirmed,
                    role = ur.RoleName
                })
                .ToListAsync();

            if (usersWithRoles.Count == 0)
            {
                _logger.Warn("No users were found in the database.");
                return NotFound("No users were found in the database.");
            }

            var filteredUsers = usersWithRoles;

            if (!string.IsNullOrEmpty(filter))
            {
                filteredUsers = UsersFiltered(usersWithRoles, filter);
            }

            var pagedUsers = filteredUsers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(pagedUsers);
        }
        */
 
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers(string? filter)
        {
            var usersWithRoles = await _context.Users
                .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { User = u, UserRole = ur })
                .Join(_context.Roles, ur => ur.UserRole.RoleId, r => r.Id, (ur, r) => new { User = ur.User, RoleName = r.Name })
                .Select(ur => new IdentityModel
                {
                    id = ur.User.Id,
                    fullname = ur.User.FullName,
                    email = ur.User.Email,
                    username = ur.User.UserName,
                    avatar = "",
                    active = ur.User.EmailConfirmed,
                    role = ur.RoleName
                })
                .ToListAsync();

            if (usersWithRoles.Count == 0)
            {
                _logger.Warn("No users were found in the database.");
                return NotFound("No users were found in the database.");
            }

            var filteredUsers = usersWithRoles;

            if (!string.IsNullOrEmpty(filter))
            {
                filteredUsers = UsersFiltered(usersWithRoles, filter);
            }


            return Ok(filteredUsers);
        }

        /// <summary>
        /// Filters a list of users by their full name.
        /// </summary>
        /// <param name="users">The list of users to filter.</param>
        /// <param name="filter">The filter string to search for.</param>
        /// <returns>A list of users whose full name contains the filter string.</returns>

        private List<IdentityModel> UsersFiltered(List<IdentityModel> users, string filter)
        {
            return users.Where(user => user.fullname.ToLower().Contains(filter.ToLower())).ToList();
        }

        /// <summary>
        /// Retrieves a list of all roles.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing an IActionResult with the list of roles.</returns>

        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles.Select(r => r.Name).ToListAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Deletes a user from the database by their ID
        /// </summary>
        /// <param name="userId">The ID of the user to be deleted</param>
        /// <returns> HTTP response indicating the success or failure of the deletion</returns>

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                _logger.Warn($"User with ID {userId} was not found.");
                return NotFound($"User with ID {userId} was not found.");
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.Info($"User with ID {userId} has been successfully removed.");
                return Ok($"User with ID {userId} has been successfully removed.");
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred while removing user with ID {userId}: {ex.Message}");
                return StatusCode(500, $"An error occurred while removing user with ID {userId}. Please try again later.");
            }
        }

        /// <summary>
        /// Adds a new user to the database
        /// </summary>
        /// <param name="identityModel">The model containing the details of the user to be added</param>
        /// <returns> HTTP response indicating the success or failure of the addition.</returns>

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(IdentityModel identityModel)
        {
            try
            {
                var roleId = await GetRoleId(identityModel.role);
                if (roleId == null)
                {
                    _logger.Warn("Role not found.");
                    return NotFound("Role not found.");
                }

                var user = CreateUser(identityModel);

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                var userId = await GetUserIdByEmail(identityModel.email);
                await AddUserRole(userId, roleId);

                _logger.Info($"User added: {userId}");
                return CreatedAtAction(nameof(AddUser), new { id = userId }, user);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while adding user: {ex.Message}");
                return StatusCode(500, $"Error occurred while adding user: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the details of an existing user in the database
        /// </summary>
        /// <param name="identityModel">The model containing the updated details of the user</param>
        /// <returns> HTTP response indicating the success or failure of the update.</returns>

        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser(IdentityModel identityModel)
        {
            try
            {
                var user = await GetUserById(identityModel.id);
                if (user == null)
                {
                    _logger.Warn($"User with ID {identityModel.id} not found.");
                    return NotFound($"User with ID {identityModel.id} not found.");
                }

                UpdateUserDetails(user, identityModel);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var roleId = await GetRoleId(identityModel.role);
                if (roleId == null)
                {
                    _logger.Warn("Role not found.");
                    return NotFound("Role not found.");
                }

                await UpdateUserRole(user.Id, roleId);

                _logger.Info($"User with ID {identityModel.id} has been successfully updated.");
                return Ok($"User with ID {identityModel.id} has been successfully updated.");
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred while updating user with ID {identityModel.id}: {ex.Message}");
                return StatusCode(500, $"An error occurred while updating user with ID {identityModel.id}. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves a user by their ID from the database
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve</param>
        /// <returns>The user with the specified ID</returns>
        private async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        /// <summary>
        /// Updates the details of a user based on the provided identity model
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <param name="identityModel">The identity model containing updated user details</param>

        private void UpdateUserDetails(ApplicationUser user, IdentityModel identityModel)
        {
            user.FullName = identityModel.fullname;
            user.UserName = identityModel.username;
            user.Email = identityModel.email;
            user.EmailConfirmed = identityModel.active;
            user.NormalizedEmail = identityModel.email.ToUpper();
            user.NormalizedUserName = identityModel.username.ToUpper();
        }

        /// <summary>
        /// Updates the role of a user in the database
        /// </summary>
        /// <param name="userId">The ID of the user to update the role for</param>
        /// <param name="roleId">The ID of the role to assign to the user</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task UpdateUserRole(string userId, string roleId)
        {
            var userRoles = await _context.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();

            if (userRoles.Count != 0)
            {
                _context.UserRoles.RemoveRange(userRoles);
            }

            _context.UserRoles.Add(new IdentityUserRole<string> { UserId = userId, RoleId = roleId });
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves the ID of a role based on its name
        /// </summary>
        /// <param name="roleName">The name of the role to retrieve the ID for</param>
        /// <returns>The ID of the role with the specified name</returns>
        private async Task<string> GetRoleId(string roleName)
        {
            return await _context.Roles
                    .Where(r => r.Name == roleName)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new user based on the provided identity model
        /// </summary>
        /// <param name="identityModel">The identity model containing user details</param>
        /// <returns>A new ApplicationUser instance</returns>
        private ApplicationUser CreateUser(IdentityModel identityModel)
        {
            return new ApplicationUser
            {
                FullName = identityModel.fullname,
                UserName = identityModel.username,
                Email = identityModel.email,
                EmailConfirmed = identityModel.active,
                NormalizedEmail = identityModel.email.ToUpper(),
                NormalizedUserName = identityModel.username.ToUpper(),
            };
        }

        /// <summary>
        /// Retrieves the ID of a user based on their email address
        /// </summary>
        /// <param name="email">The email address of the user to retrieve the ID for</param>
        /// <returns>The ID of the user with the specified email address</returns>
        private async Task<string> GetUserIdByEmail(string email)
        {
            return await _context.Users
                        .Where(u => u.Email == email)
                        .Select(u => u.Id)
                        .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Adds a role to a user in the database
        /// </summary>
        /// <param name="userId">The ID of the user to assign the role to</param>
        /// <param name="roleId">The ID of the role to assign to the user</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task AddUserRole(string userId, string roleId)
        {
            _context.UserRoles.Add(new IdentityUserRole<string> { UserId = userId, RoleId = roleId });
            await _context.SaveChangesAsync();
        }


    }
}
