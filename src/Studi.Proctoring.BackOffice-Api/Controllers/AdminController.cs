using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Studi.Proctoring.BackOffice_Api.Models.VM;
using Studi.Proctoring.BackOffice_Api.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Microsoft.Extensions.Logging;
using Studi.Proctoring.BackOffice_Api.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Studi.Proctoring.BackOffice_Api
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "Administrators")]
    [SwaggerResponse(statusCode: 401, type: typeof(string), description: "unauthorized")]
    public class AdminController : ControllerBase
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IAdminUserService adminUserService;
        private readonly IAdminTokenService adminTokenService;

        /// <summary>
        /// Defines the logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="userService">The formsRepository<see cref="IUserService"/>.</param>
        public AdminController(IServiceProvider serviceProvider, ILogger<AdminController> logger)
        {
            adminUserService = serviceProvider.AdminUserService();
            adminTokenService = serviceProvider.AdminTokenService();
            _logger = logger;
        }    

        /// <summary>
        /// CreateAdminUserAsync creates the new admin user with the specified login/password
        /// </summary>
        /// <returns>AdminUsersVM</returns>
        [HttpPost]
        [Route("users/create")]
        [SwaggerOperation("CreateAdminUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(AdminUserVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> CreateAdminUserAsync(AdminBasicLoginVM credentials)
        {
            var authorizedUser = adminTokenService.GetUserLogin(HttpContext.User);
            if (string.IsNullOrEmpty(authorizedUser))
                return BadRequest("Authorized user email is not set.");

            var adminUserDto = await adminUserService.CreateUserAsync(credentials.Login, credentials.Password, authorizedUser);
            if (adminUserDto is null)
            {
                _logger.LogError($"Error on {nameof(CreateAdminUserAsync)}: unable to create the user {credentials.Login}.");
                return BadRequest(new { errorText = "Invalid login or password." });
            }

            _logger.LogInformation($"Successfully called {nameof(CreateAdminUserAsync)} which created user: '{credentials.Login}'.");

            return Ok(adminUserDto.ToVM());
        }

        /// <summary>
        /// Get GetAdminUsersSorted Filtered Paginated and Sorted
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAdminUsersSorted")]
        [SwaggerOperation("GetAdminUsersFilteredPaginatedSorted")]
        [SwaggerResponse(statusCode: 200, type: typeof(AdminUsersPageVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> GetAdminUsersFilteredPaginatedSortedAsync(int PageSize, int PageIndex, string searchedString, string orderByPropertyName, bool sortDescending)
        {
            // Fetch all exams meating the requirements
            var adminsPageDto = await adminUserService.GetAdminUsersFilteredPaginatedAsync(PageSize, PageIndex, searchedString, orderByPropertyName, (SortDirection)Convert.ToInt32(sortDescending));

            // Convert DTOs to VMs
            var page = new List<AdminUserVM>();
            foreach (var adminUserDto in adminsPageDto.Page)
                page.Add(adminUserDto.ToVM());

            _logger.LogInformation($"Successfully called {nameof(GetAdminUsersFilteredPaginatedSortedAsync)}: returns page {adminsPageDto.PageIndex} from {adminsPageDto.TotalItemsCount} items.");

            return Ok(new AdminUsersPageVM
            {
                PageIndex = adminsPageDto.PageIndex,
                TotalItemsCount = adminsPageDto.TotalItemsCount,
                Page = page
            });
        }

        /// <summary>
        /// DeleteAdminUserAsync deletes the specified admin user.
        /// </summary>
        /// <returns>The deleted user Id is returned.</returns>
        [HttpPost]
        [Route("users/{userId}/delete")]
        //[SwaggerOperation("DeleteAdminUserById")]
        [SwaggerResponse(statusCode: 200, type: typeof(int), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> DeleteAdminUserByIdAsync(int userId)
        {
            var authorizedUser = adminTokenService.GetUserLogin(HttpContext.User);
            if (string.IsNullOrEmpty(authorizedUser))
                return BadRequest("Authorized user email is not set.");

            var deletedAdminUserId = await adminUserService.DeleteUserAsync(userId, authorizedUser);

            _logger.LogInformation($"Successfully called {nameof(DeleteAdminUserByIdAsync)} by {authorizedUser}: delete user with id {userId}.");

            return Ok(deletedAdminUserId);
        }

        /// <summary>
        /// DeleteAdminUsersAsync deletes the specified admin users.
        /// </summary>
        /// <returns>The list of deleted user Id is returned.</returns>
        [HttpPost]
        [Route("users/delete")]
        [SwaggerOperation("DeleteAdminUsers")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<int>), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> DeleteAdminUsersAsync(List<int> userIds)
        {
            var authorizedUser = adminTokenService.GetUserLogin(HttpContext.User);
            if (string.IsNullOrEmpty(authorizedUser))
                return BadRequest("Authorized user email is not set.");

            var deletedAdminUserIds = await adminUserService.DeleteUsersAsync(userIds, authorizedUser);

            _logger.LogInformation($"Successfully called {nameof(DeleteAdminUsersAsync)} by {authorizedUser}: delete users with ids {string.Join(", ", userIds)}.");

            return Ok(deletedAdminUserIds);
        }
    }
}
