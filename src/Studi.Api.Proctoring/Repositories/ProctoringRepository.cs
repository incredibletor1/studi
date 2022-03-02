using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Studi.Api.Proctoring.Helpers;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Repositories
{
    public class ProctoringRepository : IProctoringRepository
    {
        private readonly IProctoringContext _proctoringContext;

        public ProctoringRepository(IProctoringContext proctoringContext, IMapper mapper)
        {
            this._proctoringContext = proctoringContext;
            ConversionExtension.InitMapper(mapper);
        }

        /// <summary>
        /// Get last proctoring image on userExamSession
        /// </summary>
        /// <param name="userExamSessionId"></param>
        public async Task<ProctoringImageDto> GetLastProctoringImageOfUserExamSessionAsync(int userExamSessionId)
        {
            var image = await _proctoringContext.ProctoringImages.AsNoTracking()
                        .Where(IsntDeletedProctoringImage)
                        .Where(image => image.UserExamSessionId == userExamSessionId)
                        .OrderBy(img => img.Order)
                        .LastOrDefaultAsync();
            return image.ToDto();
        }

        /// <summary>
        /// Create ProctoringImage by its uploadImage, user, session and container information
        /// </summary>
        /// <param name="proctoringImageDto"></param>
        public async Task CreateProctoringImageAsync(ProctoringImageDto proctoringImage)
        {
            // Creating new entity
            var newProctoringImageEntity = proctoringImage.ToEntity();
            newProctoringImageEntity.DateCreate = DateTime.Now;
            newProctoringImageEntity.UserCreate = GlobalConst.ProctoringApiUserName;

            // Save it to database
            await _proctoringContext.ProctoringImages.AddAsync(newProctoringImageEntity);
            await _proctoringContext.SaveChangesAsync();
        }

        public async Task CreateUserCheckImageAsync(UserImageCheckDto userImageCheck)
        {
            // Creating new entity
            var newUserImageCheckEntity = userImageCheck.ToEntity();
            newUserImageCheckEntity.DateCreate = DateTime.Now;
            newUserImageCheckEntity.UserCreate = GlobalConst.ProctoringApiUserName;

            // Save it to database
            await _proctoringContext.UserImageChecks.AddAsync(newUserImageCheckEntity);
            await _proctoringContext.SaveChangesAsync();
        }

        private readonly Expression<Func<ProctoringImage, bool>> IsntDeletedProctoringImage = (proctoringImage) => (proctoringImage.DateDelete == null || proctoringImage.DateDelete > DateTime.Now);
    }
}

