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
    [Authorize(Policy = "Users")]
    [SwaggerResponse(statusCode: 401, type: typeof(string), description: "unauthorized")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IExamSessionService examSessionService;
        private readonly IAdminUserService adminUserService;
        private readonly IAdminTokenService adminTokenService;
        private readonly IArchivationService archivationService;
        private readonly IUserExamSessionService userExamSessionService;
        private readonly IProctoringService proctoringService;

        /// <summary>
        /// Defines the logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        public UserController(IServiceProvider serviceProvider, ILogger<UserController> logger)
        {
            examSessionService = serviceProvider.ExamSessionService();
            adminUserService = serviceProvider.AdminUserService();
            adminTokenService = serviceProvider.AdminTokenService();
            archivationService = serviceProvider.ArchivationService();
            userExamSessionService = serviceProvider.UserExamSessionService();
            proctoringService = serviceProvider.ProctoringService();
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

        [HttpPost]
        [Route("ChangePassword")]
        [AllowAnonymous]
        [SwaggerOperation("ChangePassword")]
        [SwaggerResponse(statusCode: 200, type: typeof(AdminPasswordChangeVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> ChangePasswordAsync(AdminPasswordChangeVM passwordChangeVM)
        {
            await adminUserService.ChangePasswordAsync(passwordChangeVM.Login, passwordChangeVM.Password, passwordChangeVM.NewPassword);

            _logger.LogInformation($"{nameof(ChangePasswordAsync)}: user {passwordChangeVM.Login} changed password.");

            return Ok();
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

            _logger.LogInformation($"Successfully called {nameof(GetExamsSessionsFilteredPaginatedSortedAsync)}: returns page {examsPageDto.PageIndex} from {examsPageDto.TotalItemsCount} items.");

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

            _logger.LogInformation($"Successfully called {nameof(GetExamsSessionsFilteredPaginatedNotSortedAsync)}: returns page {examsPageDto.PageIndex} from {examsPageDto.TotalItemsCount} items.");

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

            _logger.LogInformation($"Successfully called {nameof(GetUserExamSessionsFilteredPaginatedByExamSessionIdAsync)} for exam '{userExamGeneralInfosDtos.ExamSessionName}': returns page {userExamGeneralInfosDtos.PageIndex} from {userExamGeneralInfosDtos.TotalItemsCount} items.");

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
            // Fetch user's exam detail infos
            var userExamDetailInfosDto = await userExamSessionService.GetUserExamDetailsByUserExamSessionIdAsync(userExamSessionId);

            _logger.LogInformation($"Successfully called {nameof(GetUserExamDetailsByUserExamAsync)} for exam id {userExamSessionId}.");

            // Convert DTO to VM
            var result = userExamDetailInfosDto.ToVM();
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
            //// Retrieves userLogin from token
            var userEmail = adminTokenService.GetUserLogin(HttpContext.User);

            await archivationService.ArchivateExamSessionByIdAsync(examSessionId, userEmail);

            _logger.LogInformation($"Successfully called {nameof(ArchivateExamSessionByIdAsync)} for exam id {examSessionId}");

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
            //// Retrieves userLogin from token
            var userEmail = adminTokenService.GetUserLogin(HttpContext.User);

            await archivationService.UnarchivateExamSessionByIdAsync(examSessionId, userEmail);

            _logger.LogInformation($"Successfully called {nameof(UnarchivateExamSessionByIdAsync)} for exam id {examSessionId}");
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
            //// Retrieves userLogin from token
            var userEmail = adminTokenService.GetUserLogin(HttpContext.User);

            await archivationService.UnarchivateExamSessionListAsync(examSessionIds, userEmail);

            _logger.LogInformation($"Successfully called {nameof(UnarchivateExamSessionListAsync)} for exams ids: {string.Join(", ", examSessionIds)}");

            return Ok();
        }

        [HttpGet]
        [Route("GetProctoringImagesPaginatedByUserExam")]
        [SwaggerOperation("GetProctoringImagesPaginatedByUserExam")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProctoringImageForUserExamPageVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public virtual async Task<IActionResult> GetProctoringImagesPaginatedByUserExamAsync(int userExamSessionId, int pageSize, int pageIndex)
        {
            // Fetch user's exam detail infos
            var proctoringImageForUserExamPageDto = await proctoringService.GetProctoringImagesPaginatedByUserExam(userExamSessionId, pageSize, pageIndex);

            // Convert DTOs to VMs
            var proctoringImagesForUserExamPage = new List<ProctoringImageForUserExamVM>();
            foreach (var proctoringImageForUserExamDto in proctoringImageForUserExamPageDto.ProctoringImagesPage)
                proctoringImagesForUserExamPage.Add(proctoringImageForUserExamDto.ToVM());

            _logger.LogInformation($"Successfully called {nameof(GetProctoringImagesPaginatedByUserExamAsync)} for user's exam id {userExamSessionId}: returns page {proctoringImageForUserExamPageDto.PageIndex} from {proctoringImageForUserExamPageDto.TotalItemsCount} items.");

            return Ok(new ProctoringImageForUserExamPageVM
            {
                PageIndex = proctoringImageForUserExamPageDto.PageIndex,
                TotalItemsCount = proctoringImageForUserExamPageDto.TotalItemsCount,
                ProctoringImagesPage = proctoringImagesForUserExamPage
            });
        }
    }
}
