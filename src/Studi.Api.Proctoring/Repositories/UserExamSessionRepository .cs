using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Studi.Api.Proctoring.Helpers;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Repositories
{
    public class UserExamSessionRepository : IUserExamSessionRepository
    {
        private readonly IProctoringContext _proctoringContext;

        public UserExamSessionRepository(IProctoringContext proctoringContext, IMapper mapper)
        {
            this._proctoringContext = proctoringContext;
            ConversionExtension.InitMapper(mapper);
        }

        /// <summary>
        /// GetUserExamSessionEntityByIdAsync
        /// </summary>
        /// <param name="id">the userExamSession id</param>
        /// <returns>returns the corresponding userExamSession, throw exception if not found</returns>
        private async Task<UserExamSession> GetUserExamSessionEntityByIdAsync(int id)
        {
            var userExamSession = await _proctoringContext.UserExamSessions
                .Where(IsntDeleted)
                .FirstOrDefaultAsync(session => session.Id == id);

            if (userExamSession is null)
                throw new ArgumentException($"no userExamSession with id {id}");
            else
                return userExamSession;
        }

        /// <summary>
        /// Get a userExamSession by its id
        /// </summary>
        /// <param name="id">the userExamSession id</param>
        /// <returns>returns the corresponding userExamSession</returns>
        public async Task<UserExamSessionDto> GetUserExamSessionByIdAsync(int id)
        {
            var userExamSession = await GetUserExamSessionEntityByIdAsync(id);

            return userExamSession.ToDto();
        }

        /// <summary>
        /// GetUserExamSession
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="evaluationCode"></param>
        /// <param name="promotionCode"></param>
        /// <param name="filiereCode"></param>
        /// <returns></returns>
        public async Task<UserExamSessionDto> GetUserExamSessionAsync(int lmsUserId, string evaluationCode, string promotionCode, string filiereCode)
        {
            var userExamSession = await _proctoringContext.UserExamSessions.Include(e => e.User).Include(e => e.ExamSession).AsNoTracking()
                .Where(userExam => userExam.User.LmsUserId == lmsUserId
                && userExam.ExamSession.RessourceCode == evaluationCode
                && userExam.ExamSession.PromotionCode == promotionCode
                && userExam.ExamSession.FiliereCode == filiereCode)
                .Where(IsntDeleted)
                .FirstOrDefaultAsync();

            return userExamSession.ToDto();
        }

        /// <summary>
        /// Create user by userExamSession model
        /// </summary>
        /// <param name="userExamSession">the userExamSession model</param>
        public async Task<UserExamSessionDto> CreateUserExamSessionAsync(UserExamSessionDto userExamSession)
        {
            var userExamSessionEntity = userExamSession.ToEntity();
            userExamSessionEntity.UserCreate = GlobalConst.ProctoringApiUserName;
            userExamSessionEntity.DateCreate = DateTime.Now;

            await _proctoringContext.UserExamSessions.AddAsync(userExamSessionEntity);
            await _proctoringContext.SaveChangesAsync();
            return userExamSessionEntity.ToDto();
        }

        /// <summary>
        /// Update userExamSession by userExamSession model
        /// </summary>
        /// <param name="userExamToUpdate">the userExamSession model</param>
        public async Task UpdateUserExamSessionAsync(UserExamSessionDto userExamToUpdate)
        {
            var originalUserExamSession = await _proctoringContext.UserExamSessions
                .Where(userExam => userExam.UserId == userExamToUpdate.UserId && userExam.SessionExamId == userExamToUpdate.SessionExamId)
                .Where(IsntDeleted)
                .SingleOrDefaultAsync();

            if (originalUserExamSession is null)
                throw new ArgumentException($"Exam session for user id {userExamToUpdate.UserId} and sessionExam id {userExamToUpdate.SessionExamId} doesn't exists");

            // Update entity properties from DTO
            // TODO: complete to all properties (now update is limited to: end user exam, update exam materialCheck)
            originalUserExamSession.StatusId = userExamToUpdate.StatusId; 
            originalUserExamSession.ActualStartTime = userExamToUpdate.ActualStartTime; //TODO: later this info has to be provided (as ActualStartTime), currently set during materialCheck
            originalUserExamSession.ActualEndTime = userExamToUpdate.ActualEndTime;
            originalUserExamSession.ExamActualDuration = userExamToUpdate.ExamActualDuration;
            originalUserExamSession.HasUserConnectionBeenTested = userExamToUpdate.HasUserConnectionBeenTested;
            originalUserExamSession.HasUserIdentityDocBeenProvided = userExamToUpdate.HasUserIdentityDocBeenProvided;
            originalUserExamSession.HasUserPictureBeenProvided = userExamToUpdate.HasUserPictureBeenProvided;
            originalUserExamSession.IdentityDocumentType = userExamToUpdate.IdentityDocumentType;
            originalUserExamSession.UploadSpeedTest = userExamToUpdate.UploadSpeedTest;
            originalUserExamSession.DownloadSpeedTest = userExamToUpdate.DownloadSpeedTest;
            originalUserExamSession.ConnectionQuality = userExamToUpdate.ConnectionQuality;
            originalUserExamSession.HasMicrophone = userExamToUpdate.HasMicrophone;
            originalUserExamSession.HasWebcam = userExamToUpdate.HasWebcam;
            originalUserExamSession.UserInfrastructure = userExamToUpdate.UserInfrastructure;
            //
            originalUserExamSession.DateUpdate = DateTime.Now;
            originalUserExamSession.UserUpdate = GlobalConst.ProctoringApiUserName;
            
            // Save the changes to DB
            await _proctoringContext.SaveChangesAsync();
        }

        private readonly Expression<Func<UserExamSession, bool>> IsntDeleted = (userExamSession) => (userExamSession.DateDelete == null || userExamSession.DateDelete > DateTime.Now);
    }
}
