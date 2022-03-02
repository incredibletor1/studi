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
using Studi.Proctoring.BackOffice_Api.Helpers;

namespace Studi.Proctoring.BackOffice_Api
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "Users")]
    [SwaggerResponse(statusCode: 401, type: typeof(string), description: "unauthorized")]
    public class GlobalSettingsController : ControllerBase
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IGlobalSettingService globalSettingService;
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
        public GlobalSettingsController(IServiceProvider serviceProvider, ILogger<AdminController> logger)
        {
            globalSettingService = serviceProvider.GlobalSettingService();
            adminUserService = serviceProvider.AdminUserService();
            adminTokenService = serviceProvider.AdminTokenService();
            _logger = logger;
        }

        #region Get endpoints

        [HttpGet]
        [Route("all")]
        [AllowAnonymous]
        [SwaggerOperation("GetAllGlobalSettings")]
        [SwaggerResponse(statusCode: 200, type: typeof(GlobalSettingsVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> GetAllGlobalSettingsAsync()
        {
            var settingsDtos = await globalSettingService.GetAllGlobalSettingsAsync();

            // Convert DTOs to VMs
            var globalSettings = new GlobalSettingsVM();
            foreach (var settingDto in settingsDtos)
            {
                globalSettings.Add(settingDto.ToVM());
            }

            _logger.LogInformation($"{nameof(GetAllGlobalSettingsAsync)}: successfully fetched all settings.");

            return Ok(globalSettings);
        }

        [HttpGet]
        [Route("interval")]
        [AllowAnonymous]
        [SwaggerOperation("GetGlobalSettingInterval")]
        [SwaggerResponse(statusCode: 200, type: typeof(GlobalSettingVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> GetProctoringImageIntervalValueAsync()
        {
            var settingDto = await globalSettingService.GetProctoringImageIntervalAsync();

            // Convert DTO to VM
            var globalSetting = settingDto.ToVM();
            
            _logger.LogInformation($"{nameof(GetProctoringImageIntervalValueAsync)}: successfully fetched setting: proctoring image interval.");

            return Ok(globalSetting);
        }

        [HttpGet]
        [Route("MaxCheckImageSize")]
        [AllowAnonymous]
        [SwaggerOperation("GetGlobalSettingMaxCheckImageSize")]
        [SwaggerResponse(statusCode: 200, type: typeof(GlobalSettingVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> GetMaxCheckImageSizeAsync()
        {
            var settingDto = await globalSettingService.GetMaxCheckImageSizeAsync();

            // Convert DTO to VM
            var globalSetting = settingDto.ToVM();

            _logger.LogInformation($"{nameof(GetMaxCheckImageSizeAsync)}: successfully fetched setting: Max Check Image Size.");

            return Ok(globalSetting);
        }

        [HttpGet]
        [Route("MaxProctoringImageSize")]
        [AllowAnonymous]
        [SwaggerOperation("GetGlobalSettingMaxProctoringImageSize")]
        [SwaggerResponse(statusCode: 200, type: typeof(GlobalSettingVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> GetMaxProctoringImageSizeAsync()
        {
            var settingDto = await globalSettingService.GetMaxProctoringImageSizeAsync();

            // Convert DTO to VM
            var globalSetting = settingDto.ToVM();

            _logger.LogInformation($"{nameof(GetMaxProctoringImageSizeAsync)}: successfully fetched setting: Max Proctoring Image Size.");

            return Ok(globalSetting);
        }

        [HttpGet]
        [Route("PasswordValidityDuration")]
        [AllowAnonymous]
        [SwaggerOperation("GetGlobalSetting")]
        [SwaggerResponse(statusCode: 200, type: typeof(GlobalSettingVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> GetPasswordValidityDurationAsync()
        {
            var settingDto = await globalSettingService.GetPasswordValidityDurationAsync();

            // Convert DTO to VM
            var globalSetting = settingDto.ToVM();

            _logger.LogInformation($"{nameof(GetPasswordValidityDurationAsync)}: successfully fetched setting: Password Validity Duration.");

            return Ok(globalSetting);
        }

        #endregion

        #region Update endpoints

        [HttpPost]
        [Route("MaxCheckImageSize")]
        [AllowAnonymous]
        [SwaggerOperation("UpdateGlobalSettingValueOfMaxCheckImageSize")]
        [SwaggerResponse(statusCode: 200, type: typeof(GlobalSettingVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(decimal), description: "bad request")]
        public async virtual Task<IActionResult> UpdateValueOfMaxCheckImageSizeAsync(decimal newValue)
        {
            // Retrieves email of the connected user via its token
            var userEmail = adminTokenService.GetUserLogin(HttpContext.User);

            var settingDto = await globalSettingService.UpdateValueOfMaxCheckImageSizeAsync(newValue, userEmail);

            _logger.LogInformation($"{nameof(UpdateValueOfMaxCheckImageSizeAsync)}: successfully update value for setting: MaxCheckImageSize.");

            return Ok(settingDto.ToVM());
        }

        [HttpPost]
        [Route("MaxProctoringImageSize")]
        [AllowAnonymous]
        [SwaggerOperation("UpdateGlobalSettingValueOfMaxProctoringImageSize")]
        [SwaggerResponse(statusCode: 200, type: typeof(GlobalSettingVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(decimal), description: "bad request")]
        public async virtual Task<IActionResult> UpdateValueOfMaxProctoringImageSizeAsync(decimal newValue)
        {
            // Retrieves email of the connected user via its token
            var userEmail = adminTokenService.GetUserLogin(HttpContext.User);

            var settingDto = await globalSettingService.UpdateValueOfMaxProctoringImageSizeAsync(newValue, userEmail);

            _logger.LogInformation($"{nameof(UpdateValueOfMaxProctoringImageSizeAsync)}: successfully update value for setting: MaxProctoringImageSize.");

            return Ok(settingDto.ToVM());
        }

        [HttpPost]
        [Route("PasswordValidityDuration")]
        [AllowAnonymous]
        [SwaggerOperation("UpdateGlobalSettingValueOfPasswordValidityDuration")]
        [SwaggerResponse(statusCode: 200, type: typeof(GlobalSettingVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(int), description: "bad request")]
        public async virtual Task<IActionResult> UpdateValueOfPasswordValidityDurationAsync(int newValue)
        {
            // Retrieves email of the connected user via its token
            var userEmail = adminTokenService.GetUserLogin(HttpContext.User);

            var settingDto = await globalSettingService.UpdateValueOfPasswordValidityDurationAsync(newValue, userEmail);

            _logger.LogInformation($"{nameof(UpdateValueOfPasswordValidityDurationAsync)}: successfully update value for setting: PasswordValidityDuration.");

            return Ok(settingDto.ToVM());
        }
        [HttpPost]
        [Route("ProctoringImageInterval")]
        [AllowAnonymous]
        [SwaggerOperation("UpdateGlobalSettingValueOfProctoringImageInterval")]
        [SwaggerResponse(statusCode: 200, type: typeof(GlobalSettingVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(int), description: "bad request")]
        public async virtual Task<IActionResult> UpdateValueOfProctoringImageIntervalAsync(int newValue)
        {
            // Retrieves email of the connected user via its token
            var userEmail = adminTokenService.GetUserLogin(HttpContext.User);

            var settingDto = await globalSettingService.UpdateValueOfProctoringImageIntervalAsync(newValue, userEmail);

            _logger.LogInformation($"{nameof(UpdateValueOfProctoringImageIntervalAsync)}: successfully update value for setting: ProctoringImageInterval.");

            return Ok(settingDto.ToVM());
        }

        #endregion
    }
}
