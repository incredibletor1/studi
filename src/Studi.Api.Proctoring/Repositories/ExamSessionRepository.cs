using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Studi.Api.Proctoring.Helpers;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;
using System.Threading.Tasks;
using AutoMapper;
using Studi.Api.Proctoring.Models;
using Studi.Api.Proctoring.Middlewares.Exceptions;

namespace Studi.Api.Proctoring.Repositories
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
        /// <returns>returns the corresponding examSession</returns>
        public async Task<ExamSessionDto> GetExamSessionByIdAsync(int id)
        {
            var examSession = await GetExamSessionEntityByIdAsync(id);

            return examSession.ToDto();
        }

        /// <summary>
        /// Get a examSession by its evaluation code, promotion code and filiere code
        /// </summary>
        /// <param name="evaluationCode">the evaluation code</param>
        /// <param name="promotionCode">the promotion code</param>
        /// <param name="filiereCode">the filiere code</param>
        /// <returns>returns the corresponding examSession</returns>
        public async Task<ExamSessionDto> GetExamSessionAsync(string evaluationCode, string promotionCode, string filiereCode)
        {
            var examSession = await _proctoringContext.ExamSessions.AsNoTracking()
                            .Where(exam => exam.RessourceCode == evaluationCode && exam.PromotionCode == promotionCode && exam.FiliereCode == filiereCode)
                            .Where(IsntDeleted)
                            .FirstOrDefaultAsync();
            return examSession.ToDto();
        }

        /// <summary>
        /// Create examSession
        /// </summary>
        /// <param name="examSession">the ExamSession model</param>
        public async Task<ExamSessionDto> CreateExamSessionAsync(ExamSessionDto examSession) 
        {
            var examSessionEntity = examSession.ToEntity();
            examSessionEntity.DateCreate = DateTime.Now;
            examSessionEntity.UserCreate = GlobalConst.ProctoringApiUserName;
            //
            await _proctoringContext.ExamSessions.AddAsync(examSessionEntity);
            await _proctoringContext.SaveChangesAsync();
            return examSessionEntity.ToDto();
        }

        private readonly Expression<Func<ExamSession, bool>> IsntDeleted = (examSession) => (examSession.DateDelete == null || examSession.DateDelete > DateTime.Now);
    }
}
