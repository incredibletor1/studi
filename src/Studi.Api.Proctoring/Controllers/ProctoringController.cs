namespace Studi.Api.Proctoring.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using System;
    using System.Threading.Tasks;

    using Studi.Api.Proctoring.Services;
    using Studi.Api.Proctoring.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Studi.Api.Proctoring.Models.VM;
    using Microsoft.Extensions.Logging;
    using System.Globalization;

    [ApiController]
    [Route("[controller]")]
    [Authorize]
    [SwaggerResponse(statusCode: 401, type: typeof(string), description: "unauthorized")]
    public class ProctoringController : ControllerBase
    {
        /// <summary>
        /// Defines the services provider
        /// </summary>
        private readonly ITokenService tokenService;
        private readonly IProctoringService proctoringService;
        private readonly IGlobalSettingService globalSettingService;

        /// <summary>
        /// Defines the logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProctoringController"/> class.
        /// </summary>
        /// <param name="recordService">The formsRepository<see cref="IProctoringService"/>.</param>
        public ProctoringController(IServiceProvider serviceProvider, ILogger<ProctoringController> logger)
        {
            tokenService = serviceProvider.TokenService();
            proctoringService = serviceProvider.ProctoringService();
            globalSettingService = serviceProvider.GlobalSettingService();
            _logger = logger;
        }

        /// <summary>
        /// Endpoint for starting user Exam (and proctoring session)
        /// </summary>
        /// <param name="startExamVM">View model for StartExam.</param>
        [HttpPost]
        [Route("UserExamIdentityCheck")]
        [Route("UserExamProctoringSessionIdentityCheck")]
        [SwaggerOperation("UserExamIdentityCheck")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async Task<IActionResult> UserExamIdentityCheckAsync([FromForm] StartExamVM startExamVM)
        {
            var token = HttpHelper.GetLmsApiClientAuthorizationTokenFromCallerToken(HttpContext);
            var lmsApiBaseUrl = tokenService.GetIssuerFromToken(token);

            if (string.IsNullOrWhiteSpace(lmsApiBaseUrl))
                throw new UnauthorizedAccessException("Unable to retrieve issuer from provided JWT authentification token");

            lmsApiBaseUrl = new UriBuilder("https", lmsApiBaseUrl).Uri.ToString() + "ws/";
                
            var userExamId = await proctoringService.InitUserExamAndCreateIDCheckImagesAsync(startExamVM, lmsApiBaseUrl, token);

            _logger.LogInformation($"Successfully {nameof(UserExamIdentityCheckAsync)}; Email: '{startExamVM.Email}', EvaluationCode: '{startExamVM.EvaluationCode}'.");
            return Ok(userExamId);
        }

        /// <summary>
        /// Endpoint for meaterial check
        /// </summary>
        /// <param name="userExamMaterialCheckVM">View model for UserExamMaterialCheck.</param>
        [HttpPost]
        [Route("UserExamMaterialCheck")]
        [Route("UserExamProctoringSessionMaterialCheck")]
        [SwaggerOperation("UserExamMaterialCheck")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async Task<IActionResult> UserExamMaterialCheckAsync(UserExamMaterialCheckVM userExamMaterialCheckVM)
        {
            var currentUserExamSessionProctoringImagesInterval = await proctoringService.UserExamMaterialCheckAndStartUserExamAsync(userExamMaterialCheckVM.ToDto());

            _logger.LogInformation($"Successfully {nameof(UserExamMaterialCheckAsync)}; UserExamId: '{userExamMaterialCheckVM.UserExamSessionId}'.");
            return Ok(currentUserExamSessionProctoringImagesInterval);
        }

        /// <summary>
        /// Upload proctoring image for user's exam
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="evaluationCode"></param>
        /// <param name="promotionCode"></param>
        /// <param name="filiereCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("upload_image")]
        [SwaggerOperation("UploadImage")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async Task<IActionResult> UploadProctoringImageAsync([FromForm] UploadImageVM uploadImage)
        {
            // Retrieve user's LMS Id from token
            var token = HttpHelper.GetLmsApiClientAuthorizationTokenFromCallerToken(HttpContext);
            var lmsUserId = tokenService.GetUserIdFromToken(token);

            // Synchronously process and save the proctoring image before responde (to avoid server crashes on overload)
            await proctoringService.UploadProctoringImageAsync(uploadImage, lmsUserId);

            _logger.LogInformation($"Successfully {nameof(UploadProctoringImageAsync)}.");
            return Ok();
        }

        /// <summary>
        /// Endpoint for EndExam 
        /// </summary>
        /// <param name="endExamVM">View model for EndExam.</param>
        [HttpPost]
        [Route("end_user_exam")]
        [SwaggerOperation("EndUserExam")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async Task<IActionResult> EndUserExamAsync(EndExamVM endExamVM)
        {
            // Retrieve user's LMS Id from token
            var token = HttpHelper.GetLmsApiClientAuthorizationTokenFromCallerToken(HttpContext);
            var lmsUserId = tokenService.GetUserIdFromToken(token);

            await proctoringService.FinishUserExamAsync(lmsUserId, endExamVM.EvaluationCode, endExamVM.PromotionCode, endExamVM.FiliereCode);
            
            _logger.LogInformation($"Successfully {nameof(EndUserExamAsync)}");
            return Ok();
        }

        /// <summary>
        /// Endpoint for GetMaxImagesSizes 
        /// </summary>
        [HttpGet]
        [Route("MaxImagesSizes")]
        [SwaggerOperation("GetMaxImagesSizes")]
        [SwaggerResponse(statusCode: 200, type: typeof(MaxImagesSizesVM), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async Task<IActionResult> GetMaxImagesSizesAsync()
        {
            var maxCheckImageSizeDto = await globalSettingService.GetMaxCheckImageSizeAsync();
            var maxProctoringImageSizeDto = await globalSettingService.GetMaxProctoringImageSizeAsync();

            var style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            var provider = new CultureInfo("en-US");

            var maxImagesSizes = new MaxImagesSizesVM
            {
                MaxCheckImageSize = Decimal.Parse(maxCheckImageSizeDto.Value, style, provider),
                MaxProctoringImageSize = Decimal.Parse(maxProctoringImageSizeDto.Value, style, provider),
            };

            _logger.LogInformation($"Successfully {nameof(GetMaxImagesSizesAsync)}");
            return Ok(maxImagesSizes);
        }
    }
}
