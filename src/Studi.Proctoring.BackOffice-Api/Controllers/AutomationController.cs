using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Studi.Proctoring.BackOffice_Api.Models.VM;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Microsoft.Extensions.Logging;
using Studi.Proctoring.BackOffice_Api.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Studi.Proctoring.BackOffice_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "Automation service")]
    [SwaggerResponse(statusCode: 401, type: typeof(string), description: "unauthorized")]
    public class AutomationController : ControllerBase
    {
        private IHangFireService hangFireService;

        /// <summary>
        /// Defines the logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationController"/> class.
        /// </summary>
        public AutomationController(IServiceProvider serviceProvider, ILogger<AutomationController> logger)
        {
            hangFireService = serviceProvider.HangFireService();
            _logger = logger;
        }

        [HttpDelete]
        [Route("systemDeleteObsolete")]
        [SwaggerOperation("SystemDeleteObsolete")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<int>), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public virtual async Task<IActionResult> SystemDeleteObsoleteAync()
        {
            var deletedExamIds = await hangFireService.DeleteAllObsoleteExamSessionsAndUserExamSessions();

            _logger.LogInformation($"Successfully called {nameof(SystemDeleteObsoleteAync)} which deleted {deletedExamIds.Count} obsolete exams");

            return Ok(deletedExamIds);
        }

        [HttpPost]
        [Route("systemUpdateWronglyOngoingUserExamsToFinishWithError")]
        [SwaggerOperation("systemUpdateWronglyOngoingUserExamsToFinishWithError")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<int>), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public virtual async Task<IActionResult> SystemUpdateWronglyOngoingUserExamsToFinishWithErrorAsync()
        {
            var finishedWithErrorUserExamIds = await hangFireService.UpdateWronglyOngoingUserExamsToFinishWithErrorStatusAsync();

            _logger.LogInformation($"Successfully called {nameof(SystemUpdateWronglyOngoingUserExamsToFinishWithErrorAsync)} which finished with error {finishedWithErrorUserExamIds.Count} exams");

            return Ok(finishedWithErrorUserExamIds);
        }
    }
}
