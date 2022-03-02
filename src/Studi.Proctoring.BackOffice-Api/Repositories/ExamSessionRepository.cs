using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.Database.Context;
using System.Threading.Tasks;
using AutoMapper;
using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Exceptions;
using Studi.Proctoring.BackOffice_Api.Models.VM;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public class ExamSessionRepository : IExamSessionRepository
    {
        private readonly IProctoringContext _proctoringContext;

        public ExamSessionRepository(IProctoringContext proctoringContext, IMapper mapper)
        {
            this._proctoringContext = proctoringContext;
            ConversionExtension.InitMapper(mapper);
        }

        /// <summary>
        /// GetExamSessionEntityByIdAsync
        /// </summary>
        /// <param name="id">the examSession id</param>
        /// <returns>returns the corresponding examSession</returns>
        private async Task<ExamSession> GetExamSessionEntityByIdAsync(int id)
        {
            var examSession = await _proctoringContext.ExamSessions
                            .Where(session => session.Id == id)
                            .Where(IsntDeleted)
                            .FirstOrDefaultAsync();

            if (examSession is null)
                throw new ArgumentException($"no ExamSession with id {id}");
            else 
                return examSession;
        }

        /// <summary>
        /// Get a examSession by its id
        /// </summary>
        /// <param name="id">the examSession id</param>
        /// <returns>returns the corresponding examSession, throw exception if not found</returns>
        public async Task<ExamSessionDto> GetExamSessionByIdAsync(int id)
        {
            var examSession = await GetExamSessionEntityByIdAsync(id);

            return examSession.ToDto();
        }

        /// <summary>
        /// GetCountExamsSessionsFilteredAsync
        /// </summary>
        /// <param name="getArchivedExams">Condition flag to specify which sessions to return: normal or archived.</param>
        /// <param name="searchedString">Search string to filter records only containing it inside fields.</param>
        /// <returns>Count of exam sessions records meet the specified conditions.</returns>
        public async Task<int> GetCountExamsSessionsFilteredAsync(bool getArchivedExams = false, string searchedString = null)
        {
            var examsSessionsQuery = GetExamsSessionsFilteredQuery(getArchivedExams, searchedString);
            return await examsSessionsQuery.CountAsync();
        }

        /// <summary>
        /// GetExamsSessionsFilteredPaginatedAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ExamSessionView>> GetExamsSessionsFilteredPaginatedAsync(int PageSize, int PageIndex, bool getArchivedExams = false, string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending)
        {
            orderByPropertyName ??= "Id";
            // Check that the provided property name to use to order results belongs to ExamSession entity (using reflexion)
            if (typeof(ExamSessionView).GetProperty(orderByPropertyName) is null)
            {
                // Try to uppercase first letter to handle json formated properties
                orderByPropertyName = orderByPropertyName.Substring(0, 1).ToUpper() + orderByPropertyName.Substring(1);
                if (typeof(ExamSessionView).GetProperty(orderByPropertyName) is null)
                    throw new ArgumentException($"Provided property with name: '{orderByPropertyName}' on which sorting should be performed doesn't belong to {nameof(ExamSession)} entity nor table");
            }

            // Check page size and page index
            if (PageIndex <= 0) PageIndex = 1;
            if (PageSize <= 0) PageSize = 10000;

            var examsSessionsQuery = GetExamsSessionsFilteredQuery(getArchivedExams, searchedString);
            var allExamsSessions = (await examsSessionsQuery
                           .Select(es => new ExamSessionView
                           {
                               Id = es.Id,
                               FiliereCode = es.FiliereCode,
                               FiliereName = es.FiliereName,
                               PromotionCode = es.PromotionCode,
                               PromotionName = es.PromotionName,
                               ParcoursCode = es.ParcoursCode,
                               ParcoursName = es.ParcoursName,
                               EvalBlockCode = es.EvalBlockCode,
                               EvalBlockName = es.EvalBlockName,
                               RessourceCode = es.RessourceCode,
                               RessourceName = es.RessourceName,
                               RessourceVersionId = es.RessourceVersionId,
                               ExamInitialDuration = es.ExamInitialDuration,
                               ScheduledBeginStartTime = es.ScheduledBeginStartTime,
                               ScheduledEndStartTime = es.ScheduledEndStartTime,
                               IntervalProctoringImages = es.IntervalProctoringImages,
                               UsersCount = es.UserExamSessions.Count(),
                               ExamStatus = es.UserExamSessions.Any(ues => ues.StatusId == (int)ExamStatusEnum.Ongoing) ?
                                    ExamStatusEnum.Ongoing : ExamStatusEnum.Finished
                           })
                           .OrderByDynamic<ExamSessionView>(orderByPropertyName, orderByDirection)
                           .ThenByDescending(es => es.Id)
                           .Skip((PageIndex - 1) * PageSize)
                           .Take(PageSize)
                           .ToListAsync());

            return allExamsSessions;
        }

        /// <summary>
        /// GetExamSessionsForSystemDeleteAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ExamSessionDto>> GetMustBeSystemDeletedExamSessionsAsync()
        {
            var examSessions = await _proctoringContext.ExamSessions.Include(es => es.UserExamSessions)
                .Where(IsntDeleted)
                .Where(MustBeSystemDeleted)
                .Where(es => es.UserExamSessions.Any())
                .Where(es => es.UserExamSessions.All(userExamSession => userExamSession.DateDelete != null && userExamSession.DateDelete <= DateTime.Now))
                .ToListAsync();

            return examSessions.Select(es => es.ToDto());
        }

        /// <summary>
        /// SystemDeleteExamSessionByIdAsync
        /// </summary>
        public async Task DeleteExamSessionByIdAsync(int examSessionId, string userDelete)
        {
            var examSession = await GetExamSessionEntityByIdAsync(examSessionId);

            examSession.DateDelete = DateTime.Now;
            examSession.UserDelete = userDelete;

            await _proctoringContext.SaveChangesAsync();
        }

        /// <summary>
        /// ArchivateExamSessionById
        /// </summary>
        /// <param name="examSessionId"></param>
        public async Task ArchivateExamSessionByIdAsync(int examSessionId, string userArchive) 
        {
            var examSession = await GetExamSessionEntityByIdAsync(examSessionId);

            examSession.DateArchive = DateTime.Now;
            examSession.UserArchive = userArchive;
            //
            examSession.DateUpdate = DateTime.Now;
            examSession.UserUpdate = userArchive;

            await _proctoringContext.SaveChangesAsync();
        }

        /// <summary>
        /// UnarchivateExamSessionByIdAsync
        /// </summary>
        /// <param name="examSessionId"></param>
        public async Task UnarchivateExamSessionByIdAsync(int examSessionId, string userUnarchive) 
        {
            var examSession = await GetExamSessionEntityByIdAsync(examSessionId);

            examSession.DateArchive = null;
            examSession.UserArchive = null;
            //
            examSession.DateUpdate = DateTime.Now;
            examSession.UserUpdate = userUnarchive;

            await _proctoringContext.SaveChangesAsync();
        }

        public async Task CheckPermissionToArchivateExamSessionByIdAsync(int examSessionId)
        {
            var aggregateArchivingPermissionExceptions = new List<CannotBeArchivedException>();

            var examSession = await _proctoringContext.ExamSessions.Include(ex => ex.UserExamSessions)
                .Where(IsntDeleted)
                .Where(IsntArchived)
                .Where(IsFinishedBySchedule)
                .FirstOrDefaultAsync(es => es.Id == examSessionId);

            if (examSession is null)
                aggregateArchivingPermissionExceptions.Add(new CannotBeArchivedException($"ExamSession with id {examSessionId} cannot be archived because it is already: archived or deleted or hasn't ended yet"));

            bool IsOngoingOrUnfinished(UserExamSession userExamSession)
            {
                return IsOngoing(userExamSession) || IsntFinished(userExamSession);
            }

            var userExamSessions = examSession.UserExamSessions
                .Where(IsntDeletedUserSession)
                .Where(IsntArchivedUserSession)
                .Where(IsOngoingOrUnfinished);

            if (userExamSessions.Any())
            {
                var ongoingUserExamSessions = userExamSessions.Where(IsOngoing);
                if (ongoingUserExamSessions.Any())
                    aggregateArchivingPermissionExceptions.Add(new CannotBeArchivedException($"ExamSession with id {examSessionId} cannot be archived because UserExamSessions with id" +
                $" {string.Join(",", ongoingUserExamSessions.Select(ues => ues.Id))} still {ExamStatusEnum.Ongoing}"));

                var scheduledUserExamSessions = userExamSessions.Where(ues => ues.ActualEndTime == null && ues.ScheduledSpecificEndStartTime.Value.AddMinutes(ues.ExamTotalDuration) > DateTime.Now);
                if (scheduledUserExamSessions.Any())
                    aggregateArchivingPermissionExceptions.Add(new CannotBeArchivedException($"ExamSession with id {examSessionId} cannot be archived because UserExamSessions with id" +
                $" {string.Join(",", scheduledUserExamSessions.Select(ues => ues.Id))} not finished yet on schedule"));
            }
            
            if (aggregateArchivingPermissionExceptions.Any())
            {
                throw new CannotBeArchivedAggregateException(aggregateArchivingPermissionExceptions);
            }
        }

        /// <summary>
        /// GetExamsSessionsFilteredQuery
        /// </summary>
        /// <param name="getArchivedExams">Condition flag to specify which sessions to return: normal or archived.</param>
        /// <param name="searchedString">Search string to filter records only containing it inside fields.</param>
        /// <returns>Prepared query with specified conditions, which can be used further to perform required selections.</returns>
        private IQueryable<ExamSession> GetExamsSessionsFilteredQuery(bool getArchivedExams = false, string searchedString = null)
        {
            Expression<Func<ExamSession, bool>> SearchForString = (examSession) => string.IsNullOrWhiteSpace(searchedString) ? true :
                             (examSession.RessourceName.Contains(searchedString) || examSession.RessourceCode.Contains(searchedString)
                           || examSession.EvalBlockName.Contains(searchedString) || examSession.EvalBlockCode.Contains(searchedString)
                           || examSession.ParcoursName.Contains(searchedString) || examSession.ParcoursCode.Contains(searchedString)
                           || examSession.PromotionName.Contains(searchedString) || examSession.PromotionCode.Contains(searchedString)
                           || examSession.FiliereName.Contains(searchedString) || examSession.FiliereCode.Contains(searchedString));

            var examsSessionsQuery = _proctoringContext.ExamSessions.Include(es => es.UserExamSessions)
                           .AsNoTracking()
                           .Where(IsntDeleted)
                           .Where(getArchivedExams ? IsArchived : IsntArchived)
                           .Where(SearchForString);

            return examsSessionsQuery;
        }

        private readonly Expression<Func<ExamSession, bool>> IsntDeleted = (examSession) => (examSession.DateDelete == null || examSession.DateDelete > DateTime.Now);
        private readonly Expression<Func<ExamSession, bool>> IsntArchived = (examSession) => (examSession.DateArchive == null || examSession.DateArchive > DateTime.Now);
        // For duration calculation, 1/3 time is added to initial duration to concider RQTH cases + 5 extra minutes as default deposit time
        private readonly Expression<Func<ExamSession, bool>> MustBeSystemDeleted = (examSession)
            => (examSession.ScheduledEndStartTime.AddMinutes((examSession.ExamInitialDuration * (4.0 / 3.0)) + 5.0).AddMonths(6) < DateTime.Now); 
        private readonly Func<UserExamSession, bool> IsntDeletedUserSession = (userExamSession) => (userExamSession.DateDelete == null || userExamSession.DateDelete > DateTime.Now);
        private readonly Func<UserExamSession, bool> IsntArchivedUserSession = (userExamSession) => (userExamSession.DateArchive == null || userExamSession.DateArchive > DateTime.Now);
        private readonly Func<UserExamSession, bool> IsDeletedUserSession = (userExamSession) => (userExamSession.DateDelete != null && userExamSession.DateDelete <= DateTime.Now);
        private readonly Func<UserExamSession, bool> IsOngoing = (userExamSession) => userExamSession.StatusId == (int)ExamStatusEnum.Ongoing;
        private readonly Func<UserExamSession, bool> IsntFinished = (userExamSession) => userExamSession.ActualEndTime == null && userExamSession.ScheduledSpecificEndStartTime.Value.AddMinutes(userExamSession.ExamTotalDuration) > DateTime.Now;
        private readonly Expression<Func<ExamSession, bool>> IsFinishedBySchedule = (examSession)
            => (examSession.ScheduledEndStartTime.AddMinutes((examSession.ExamInitialDuration * (4.0 / 3.0)) + 5.0) < DateTime.Now);
        private readonly Expression<Func<ExamSession, bool>> IsArchived = (examSession) => (examSession.DateArchive != null && examSession.DateArchive <= DateTime.Now);
    }
}
