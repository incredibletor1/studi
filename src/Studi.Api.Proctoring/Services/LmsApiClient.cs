using Studi.Api.Proctoring.Models.DTO;
using Studi.Api.Proctoring.Helpers;
using IO.Swagger.Api;
using System;
using System.Net.Http;

namespace Studi.Api.Proctoring.Services
{
    public class LmsApiClient : ILmsApiClient
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        public readonly IHttpClientFactory _clientFactory;

        private string _lmsApiBaseUrl = null;
        private string _authToken = null;

        public string LmsApiBaseUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_lmsApiBaseUrl))
                    throw new NotImplementedException("");
                else
                    return _lmsApiBaseUrl;
            }

            set => _lmsApiBaseUrl = value;
        }

        public string AuthToken
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_authToken))
                    throw new UnauthorizedAccessException("authorization token haven't been provided");
                else
                    return _authToken;
            }
            set => _authToken = value;
        }

        //public void SetAuthToken(string authToken)
        //{
        //    this.AuthToken = authToken;
        //}


        public LmsApiClient(IServiceProvider serviceProvider, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Get User Infos through a call to LMS.API.Client
        /// </summary>
        /// <param name="evaluationId"></param>
        /// <param name="lmsApiBaseUrl"></param>
        /// <returns></returns>
        public UserDto GetUserInfos(string evaluationCode)
        {            
            var lmsApiProctor = new ProctoringApi(LmsApiBaseUrl, AuthToken);
            var userInfosVM = lmsApiProctor.GetUserInfos(evaluationCode);
            var result = userInfosVM.ToDto();
            return result;
        }

        public ExamSessionDto GetExamInfos(int ressourceVersionId, string promotionCode, string filiereCode)
        {
            var lmsApiProctor = new ProctoringApi(LmsApiBaseUrl, AuthToken);
            var examInfosVM = lmsApiProctor.GetBlockExamSessionInfos(ressourceVersionId, promotionCode, filiereCode);

            // Checks for data consistency
            if (examInfosVM.ExamInitialDuration is null)
                throw new NullReferenceException($"Fetched ExamInitialDuration value is null for ExamSession with ressourceVersion id: {ressourceVersionId}");
            if (examInfosVM.ScheduledBeginStartTime is null)
                throw new NullReferenceException($"Fetched ScheduledBeginStartTime value is null for ExamSession with ressourceVersion id: {ressourceVersionId}");
            if (examInfosVM.ScheduledEndStartTime  is null)
                throw new NullReferenceException($"Fetched ScheduledEndStartTime value is null for ExamSession with ressourceVersion id: {ressourceVersionId}");

            var result = examInfosVM.ToDto();
            return result;
        }

        public UserExamSessionDto GetUserExamInfos(string evaluationCode, string promotionCode, int userId, int examSessionId)
        {
            var lmsApiProctor = new ProctoringApi(LmsApiBaseUrl, AuthToken);
            var userExamInfosVM = lmsApiProctor.GetUserBlockExamSessionInfos(evaluationCode, promotionCode);
            var userExamDto = userExamInfosVM.ToDto();

            // WARNING: Fetched 'UserId' and 'SessionExamId' are ids of LMS database, thus, they must be overrided to ids of Proctoring database
            userExamDto.UserId = userId;
            userExamDto.SessionExamId = examSessionId;

            return userExamDto;
        }
    }
}
