﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Models.VM;
using Studi.Proctoring.Database.Context;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
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
        /// <returns>returns the corresponding userExamSession, throw exception if not found</returns>
        public async Task<UserExamSessionDto> GetUserExamSessionByIdAsync(int id)
        {
            var userExamSession = await GetUserExamSessionEntityByIdAsync(id); 
            
            return userExamSession.ToDto();
        }

        /// <summary>
        /// Delete UserExamSession by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteUserExamSessionByIdAsync(int id, string userDelete)
        {
            var userExamSession = await GetUserExamSessionEntityByIdAsync(id);

            userExamSession.DateDelete = DateTime.Now;
            userExamSession.UserDelete = userDelete;

            await _proctoringContext.SaveChangesAsync();
        }

        public async Task ArchivateUserExamSessionByIdAsync(int userExamSessionId, string userArchive) 
        {
            var userExamSession = await GetUserExamSessionEntityByIdAsync(userExamSessionId);

            userExamSession.DateArchive = DateTime.Now;
            userExamSession.UserArchive = userArchive;
            //
            userExamSession.DateUpdate = DateTime.Now;
            userExamSession.UserUpdate = userArchive;

            await _proctoringContext.SaveChangesAsync();
        }

        public async Task UnarchivateUserExamSessionByIdAsync(int userExamSessionId, string userUnarchive) 
        {
            var userExamSession = await GetUserExamSessionEntityByIdAsync(userExamSessionId);

            userExamSession.DateArchive = null;
            userExamSession.UserArchive = null;
            //
            userExamSession.DateUpdate = DateTime.Now;
            userExamSession.UserUpdate = userUnarchive;

            await _proctoringContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserExamSessionDto>> GetUserExamSessionsByExamSessionIdAsync(int examSessionId, bool getArchivedUserExams = false)
        {
            var userExamSessions = await GetQueryUserExamSessionsByExamSessionIdAsync(examSessionId)
                .Where(getArchivedUserExams ? IsArchived : IsntArchived)
                .ToListAsync();

            return userExamSessions.Select(ues => ues.ToDto());
        }

        /// <summary>
        /// GetUserExamSessionsForSystemDelete
        /// </summary>
        /// <returns>corresponding userExamSessions</returns>
        public async Task<IEnumerable<UserExamSessionDto>> GetObsoleteUserExamSessionsToDeleteAsync()
        {
            var userExamSessions = await _proctoringContext.UserExamSessions
                .AsNoTracking()
                .Where(IsntDeleted)
                .Where(CanBeDeleted)
                .Where(MustBeSystemDeleted)
                .ToListAsync();

            return userExamSessions.Select(ues => ues.ToDto());
        }

        public async Task<IEnumerable<UserExamSessionDto>> GetOngoingUserExamSessionsByExamSessionIdAsync(int examSessionId)
        {
            var userExamSessions = await GetQueryUserExamSessionsByExamSessionIdAsync(examSessionId)
                .Where(IsOngoing)
                .ToListAsync();

            return userExamSessions.Select(ues => ues.ToDto());
        }

        public async Task<int> GetCountUserExamSessionsFilteredAsync(int examSessionId, string searchedString = null)
        {
            var userExamSessionsQuery = GetUserExamSessionsFilteredQuery(examSessionId, searchedString);
            return await userExamSessionsQuery.CountAsync();
        }

        public async Task<IEnumerable<UserExamGeneralInfosDto>> GetUserExamSessionsFilteredPaginatedAsync(int examSessionId, int PageSize, int PageIndex, string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending)
        {
            orderByPropertyName ??= "Id";
            // Check that the provided property name to use to order results belongs to UserExamSessions entity (using reflexion)
            if (typeof(UserExamGeneralInfosDto).GetProperty(orderByPropertyName) is null)
            {
                // Try to uppercase first letter to handle json formated properties
                orderByPropertyName = orderByPropertyName.Substring(0, 1).ToUpper() + orderByPropertyName.Substring(1);
                if (typeof(UserExamGeneralInfosDto).GetProperty(orderByPropertyName) is null)
                    throw new ArgumentException($"Provided property with name: '{orderByPropertyName}' on which sorting should be performed doesn't belong to {nameof(UserExamGeneralInfosDto)} entity nor table");
            }

            // Check page size and page index
            if (PageIndex <= 0) PageIndex = 1;
            if (PageSize <= 0) PageSize = 10000;

            var userExamSessionsQuery = GetUserExamSessionsFilteredQuery(examSessionId, searchedString);
            var userExamSessionsPage = (await userExamSessionsQuery
                .Include(ues => ues.data_ExamStatu)
                .Select(ues => new UserExamGeneralInfosDto
                {
                    UserFirstnamePlusLastname = ues.User.FirstName + " " + ues.User.LastName,
                    UserEmail = ues.User.Email,
                    Id = ues.Id,
                    UserId = ues.UserId,
                    Status = ues.data_ExamStatu.Code,
                    HasUserConnectionBeenTested = ues.HasUserConnectionBeenTested,
                    HasUserIdentityDocBeenProvided = ues.HasUserIdentityDocBeenProvided,
                    ConnectionQuality = ues.ConnectionQuality,
                    HasMicrophone = ues.HasMicrophone,
                    HasWebcam = ues.HasWebcam
                })
                .OrderByDynamic<UserExamGeneralInfosDto>(orderByPropertyName, orderByDirection)
                .ThenByDescending(ues => ues.Id)
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync());
                           
            return userExamSessionsPage;
        }

        /// <summary>
        /// GetUserExamSessionsFilteredQuery
        /// </summary>
        /// <param name="searchedString">Search string to filter records only containing it inside fields.</param>
        /// <returns>Prepared query with specified conditions, which can be used further to perform required selections.</returns>
        private IQueryable<UserExamSession> GetUserExamSessionsFilteredQuery(int examSessionId, string searchedString = null)
        {
            Expression<Func<UserExamSession, bool>> SearchForString = (userExamSession) => string.IsNullOrWhiteSpace(searchedString) ? true :
                             ((userExamSession.User.FirstName + " " + userExamSession.User.LastName).Contains(searchedString) ||
                             ((userExamSession.User.Email).Contains(searchedString)));

            var userExamSessionQuery = _proctoringContext.UserExamSessions.Include(ues => ues.User)
                           .AsNoTracking()
                           .Where(ues => ues.SessionExamId == examSessionId)
                           .Where(IsntDeleted)
                           .Where(SearchForString);

            return userExamSessionQuery;
        }



        private IQueryable<UserExamSession> GetQueryUserExamSessionsByExamSessionIdAsync(int examSessionId)
        {
            var examsSessionsQuery = _proctoringContext.UserExamSessions
                           .AsNoTracking()
                           .Where(ues => ues.SessionExamId == examSessionId)
                           .Where(IsntDeleted);

            return examsSessionsQuery;
        }

        public async Task<IEnumerable<UserExamForDownloadDto>> GetDownloadInformationsAboutUserExamSessionsOfExamSessionByExamSessionIdAsync(int examSessionId)
        {
            var userExamSessions = await GetQueryUserExamSessionsByExamSessionIdAsync(examSessionId)
                .Include(ues => ues.User)
                .Select(ues => new UserExamForDownloadDto
                {
                    UserExamId = ues.Id,
                    UserId = ues.User.Id,
                    UserFirstName = ues.User.FirstName,
                    UserLastName = ues.User.LastName,
                    UserEmail = ues.User.Email
                })
                .ToListAsync();
                
            return userExamSessions;
        }

        private readonly Expression<Func<UserExamSession, bool>> IsntDeleted = (userExamSession) => (userExamSession.DateDelete == null || userExamSession.DateDelete > DateTime.Now);
        private readonly Expression<Func<UserExamSession, bool>> IsntArchived = (userExamSession) => (userExamSession.DateArchive == null || userExamSession.DateArchive > DateTime.Now);
        private readonly Expression<Func<UserExamSession, bool>> IsArchived = (userExamSession) => (userExamSession.DateArchive != null && userExamSession.DateArchive <= DateTime.Now);
        private readonly Expression<Func<UserExamSession, bool>> CanBeDeleted = (userExamSession) => (userExamSession.StatusId != (int)ExamStatusEnum.Ongoing);
        private readonly Expression<Func<UserExamSession, bool>> MustBeSystemDeleted = (userExamSession) =>
            ((userExamSession.ActualEndTime.HasValue && userExamSession.ActualEndTime.Value.AddMonths(6) < DateTime.Now) || // TODO: make the 6 months delay parameterized
            (!userExamSession.ActualEndTime.HasValue && userExamSession.ScheduledSpecificEndStartTime.Value.AddMinutes(userExamSession.ExamTotalDuration).AddMonths(6) < DateTime.Now));
        private readonly Expression<Func<UserExamSession, bool>> IsOngoing = (userExamSession) => userExamSession.StatusId == (int)ExamStatusEnum.Ongoing;

    }
}
