using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;

using Studi.Proctoring.BackOffice_Api.Models.VM;
using Studi.Proctoring.BackOffice_Api.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Microsoft.Extensions.Logging;
using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Studi.Proctoring.BackOffice_Api
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "Users")]
    [SwaggerResponse(statusCode: 401, type: typeof(string), description: "unauthorized")]
    public class AdminController : ControllerBase
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IExamSessionService examSessionService;
        private readonly IAdminUserService adminUserService;
        private readonly IAdminTokenService adminTokenService;
        private IHangFireService hangFireService;
        private readonly IArchivationService archivationService;
        private readonly ITokenService tokenService;
        private readonly IImageService imageService; 
        private readonly IUserExamSessionService userExamSessionService;

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
            examSessionService = serviceProvider.ExamSessionService();
            adminUserService = serviceProvider.AdminUserService();
            adminTokenService = serviceProvider.AdminTokenService();
            hangFireService = serviceProvider.HangFireService();
            archivationService = serviceProvider.ArchivationService();
            tokenService = serviceProvider.TokenService();
            userExamSessionService = serviceProvider.UserExamSessionService();
            imageService = serviceProvider.ImageService();
            _logger = logger;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [SwaggerOperation("Login")]
        [SwaggerResponse(statusCode: 200, type: typeof(AdminLoginResponseVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> LoginAsync(AdminBasicLoginVM login)
        {
            var user = await adminUserService.LoginUserAsync(login.Login, login.Password);
            if (user is null)
            {
                _logger.LogError($"Error on {nameof(LoginAsync)}: user {login.Login} has not been found.");
                return BadRequest(new { errorText = "Invalid login or password." });
            }

            string token = adminTokenService.CreateToken(user);

            var response = new AdminLoginResponseVM
            {
                AccessToken = token,
                Role = user.UserType.ToString()
            };

            _logger.LogInformation($"{nameof(LoginAsync)}: user {login.Login} has been authenticated.");
            return Ok(response);
        }

        /// <summary>
        /// Get ExamsSessions Filtered Paginated and Sorted
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetExamsSessionsSorted")]
        [SwaggerOperation("GetExamsSessionsFilteredPaginatedAndSorted")]
        [SwaggerResponse(statusCode: 200, type: typeof(ExamSessionPageVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> GetExamsSessionsFilteredPaginatedSortedAsync(int PageSize, int PageIndex, bool getArchivedExams, string searchedString, string orderByPropertyName, bool sortDescending)
        {
            // Fetch all exams meating the requirements
            var examsPageDto = await examSessionService.GetExamsSessionsFilteredPaginatedAsync(PageSize, PageIndex, getArchivedExams, searchedString, orderByPropertyName, (SortDirection)Convert.ToInt32(sortDescending));

            // Convert DTOs to VMs
            var page = new List<ExamSessionVM>();
            foreach (var examSessionDto in examsPageDto.Page)
                page.Add(examSessionDto.ToVM());

            return Ok(new ExamSessionPageVM
            {
                PageIndex = examsPageDto.PageIndex,
                TotalItemsCount = examsPageDto.TotalItemsCount,
                Page = page
            });
        }

        /// <summary>
        /// Get ExamsSessions Filtered Paginated and Sorted
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetExamsSessionsNotSorted")]
        [SwaggerOperation("GetExamsSessionsFilteredPaginatedNotSorted")]
        [SwaggerResponse(statusCode: 200, type: typeof(ExamSessionPageVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> GetExamsSessionsFilteredPaginatedNotSortedAsync(int PageSize, int PageIndex, bool getArchivedExams, string searchedString)
        {
            // Fetch all exams meating the requirements
            var examsPageDto = await examSessionService.GetExamsSessionsFilteredPaginatedAsync(PageSize, PageIndex, getArchivedExams, searchedString);

            // Convert DTOs to VMs
            var page = new List<ExamSessionVM>();
            foreach (var examSessionDto in examsPageDto.Page)
                page.Add(examSessionDto.ToVM());

            return Ok(new ExamSessionPageVM
            {
                PageIndex = examsPageDto.PageIndex,
                TotalItemsCount = examsPageDto.TotalItemsCount,
                Page = page
            });
        }

        /// <summary>
        /// GetUserExamSessions Filtered Paginated and Sorted
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ExamSession/{examSessionId}/UserExamSessions")]
        [SwaggerOperation("GetUsersExamsByExamSession")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserExamGeneralInfosPageVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public virtual async Task<IActionResult> GetUserExamSessionsFilteredPaginatedByExamSessionIdAsync(int examSessionId, int PageSize, int PageIndex, string searchedString, string orderByPropertyName, bool sortDescending)
        {
            // Fetch all userExams meeting requirements
            var userExamGeneralInfosDtos = await userExamSessionService.GetUserExamSessionsFilteredPaginatedByExamSessionIdAsync(examSessionId, PageSize, PageIndex, searchedString, orderByPropertyName, (SortDirection)Convert.ToInt32(sortDescending));

            // Convert DTOs to VMs
            var userExamsPage = new List<UserExamGeneralInfosVM>();
            foreach (var userExamGeneralInfosDto in userExamGeneralInfosDtos.UserExamsPage)
            {
                userExamsPage.Add(userExamGeneralInfosDto.ToVM());
            }

            return Ok(new UserExamGeneralInfosPageVM
            {
                ExamSessionName = userExamGeneralInfosDtos.ExamSessionName,
                PageIndex = userExamGeneralInfosDtos.PageIndex,
                TotalItemsCount = userExamGeneralInfosDtos.TotalItemsCount,
                UserExamsPage = userExamsPage
            });
        }

        [HttpGet]
        [Route("UserExamSession/{userExamSessionId:int}/details")]
        [SwaggerOperation("GetUserExamDetailsByUserExam")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserExamDetailInfosVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public virtual async Task<IActionResult> GetUserExamDetailsByUserExamAsync(int userExamSessionId)
        {
            // TODO: to be implemented
            throw new NotImplementedException();

            // Fetch user's exam detail infos
            //var usersListDtos = await examSessionService.GetUserExamDetailsByUserExamAsync(userExamSessionId);

            // Convert DTO to VM
            var result = new UserExamDetailInfosVM();
            return Ok(result);
        }

        /// <summary>
        /// Archivate examSession by id
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("ExamSession/{examSessionId}/archivate")]
        [SwaggerOperation("ArchivateExamSessionById")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public virtual async Task<IActionResult> ArchivateExamSessionByIdAsync(int examSessionId)
        {
            //// Retrieves userId from token
            //var token = HttpHelper.GetAuthorizationTokenFromCallerToken(HttpContext);
            //var userId = tokenService.GetUserIdFromToken(token);

            // Retrieves Email by userId
            //var email = (await adminUserService.GetUserByIdAsync(userId)).Login;

            await archivationService.ArchivateExamSessionByIdAsync(examSessionId, "AdminEmail");  // TODO: Must be replaced with a real email address after enabling authorization

            _logger.LogInformation($"Successfully {nameof(ArchivateExamSessionByIdAsync)} with id {examSessionId}");
            return Ok();
        }

        /// <summary>
        /// Unarchivate examSession by id
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("ExamSession/{examSessionId}/unarchivate")]
        [SwaggerOperation("UnarchivateExamSessionById")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public virtual async Task<IActionResult> UnarchivateExamSessionByIdAsync(int examSessionId)
        {
            //// Retrieves userId from token
            //var token = HttpHelper.GetAuthorizationTokenFromCallerToken(HttpContext);
            //var userId = tokenService.GetUserIdFromToken(token);

            //// Retrieves Email by userId
            //var email = (await adminUserService.GetUserByIdAsync(userId)).Login;

            await archivationService.UnarchivateExamSessionByIdAsync(examSessionId, "AdminEmail");  // TODO: Must be replaced with a real email address after enabling authorization

            _logger.LogInformation($"Successfully {nameof(UnarchivateExamSessionByIdAsync)} with id {examSessionId}");
            return Ok();
        }

        /// <summary>
        /// Unarchivate examSessions list by ids
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("examSession/unarchivate")]
        [SwaggerOperation("UnarchivateExamSessionList")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public virtual async Task<IActionResult> UnarchivateExamSessionListAsync(List<int> examSessionIds)
        {
            //// Retrieves userId from token
            //var token = HttpHelper.GetAuthorizationTokenFromCallerToken(HttpContext);
            //var userId = tokenService.GetUserIdFromToken(token);

            //// Retrieves Email by userId
            //var email = (await adminUserService.GetUserByIdAsync(userId)).Login;

            await archivationService.UnarchivateExamSessionListAsync(examSessionIds, "AdminEmail");  // TODO: Must be replaced with a real email address after enabling authorization

            _logger.LogInformation($"Successfully {nameof(UnarchivateExamSessionListAsync)}");
            return Ok();
        }

        [HttpGet]
        [Route("ExamSession/{examSessionId}/Download")]
        [SwaggerOperation("DownloadExamSessionById")]
        [SwaggerResponse(statusCode: 200, type: typeof(FileContentResult), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public virtual async Task<IActionResult> DownloadExamSessionByIdAsync(int examSessionId)
        {
            var examSessionArchivedDto = await imageService.GetExamSessionFileInformationByExamSessionIdAsync(examSessionId);

            _logger.LogInformation($"Successfully {nameof(DownloadExamSessionByIdAsync)}");
            return PhysicalFile(examSessionArchivedDto.FilePath, examSessionArchivedDto.ContentType, examSessionArchivedDto.FileName);
        }

        [HttpGet]
        [Route("systemDeleteObsolete")]
        [SwaggerOperation("SystemDeleteObsolete")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<UserVM>), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public virtual async Task<IActionResult> SystemDeleteObsoleteAync()
        {
            var deletedExamIds = await hangFireService.DeleteAllObsoleteExamSessionsAndUserExamSessions();
            return Ok(deletedExamIds);
        }

        /// <summary>
        /// CreateAdminUserAsync creates the new admin user with the specified login/password
        /// </summary>
        /// <returns>AdminUsersVM</returns>
        [HttpPost]
        [Route("users/create")]
        [SwaggerOperation("CreateAdminUser")]
        [Authorize(Policy = "Administrators")]
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

            return Ok(adminUserDto.ToVM());
        }

        /// <summary>
        /// Get GetAdminUsersSorted Filtered Paginated and Sorted
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAdminUsersSorted")]
        [SwaggerOperation("GetAdminUsersFilteredPaginatedSorted")]
        [Authorize(Policy = "Administrators")]
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
        [SwaggerOperation("DeleteAdminUserById")]
        [Authorize(Policy = "Administrators")]
        [SwaggerResponse(statusCode: 200, type: typeof(int), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> DeleteAdminUserByIdAsync(int userId)
        {
            var authorizedUser = adminTokenService.GetUserLogin(HttpContext.User);
            if (string.IsNullOrEmpty(authorizedUser))
                return BadRequest("Authorized user email is not set.");

            var deletedAdminUserId = await adminUserService.DeleteUserAsync(userId, authorizedUser);

            return Ok(deletedAdminUserId);
        }

        /// <summary>
        /// DeleteAdminUsersAsync deletes the specified admin users.
        /// </summary>
        /// <returns>The list of deleted user Id is returned.</returns>
        [HttpPost]
        [Route("users/delete")]
        [SwaggerOperation("DeleteAdminUsers")]
        [Authorize(Policy = "Administrators")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<int>), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> DeleteAdminUserAsync(List<int> userIds)
        {
            var authorizedUser = adminTokenService.GetUserLogin(HttpContext.User);
            if (string.IsNullOrEmpty(authorizedUser))
                return BadRequest("Authorized user email is not set.");

            var deletedAdminUserIds = await adminUserService.DeleteUsersAsync(userIds, authorizedUser);

            return Ok(deletedAdminUserIds);
        }
    }
}
