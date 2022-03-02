using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.Database.Context;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
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
        /// GetProctoringPicturesEntityAsync
        /// </summary>
        /// <param name="userExamSessionId"></param>
        private async Task<IEnumerable<ProctoringImage>> GetProctoringPicturesEntityAsync(int userExamSessionId)
        {
            var proctoringImages = await _proctoringContext.ProctoringImages
                .Where(IsntDeletedProctoringImage)
                .Where(img => img.UserExamSessionId == userExamSessionId)
                .ToListAsync();

            return proctoringImages;
        }

        /// <summary>
        /// GetUserCheckPicturesEntityAsync
        /// </summary>
        /// <param name="userExamSessionId"></param>
        private async Task<IEnumerable<UserImageCheck>> GetUserCheckPicturesEntityAsync(int userExamSessionId)
        {
            var userCheckImages = await _proctoringContext.UserImageChecks
                .Where(IsntDeletedUserCheckImage)
                .Where(img => img.UserExamSessionId == userExamSessionId)
                .ToListAsync();

            return userCheckImages;
        }

        /// <summary>
        /// Delete userCheckPictures by userExamSession Id
        /// </summary>
        /// <param name="userExamSessionId"></param>
        public async Task DeleteFromDBUserCheckPicturesAsync(int userExamSessionId, string userDelete)
        {
            var userCheckImages = await GetUserCheckPicturesEntityAsync(userExamSessionId);

            foreach (var userCheckImage in userCheckImages)
            {
                userCheckImage.DateDelete = DateTime.Now;
                userCheckImage.UserDelete = userDelete;
            }

            await _proctoringContext.SaveChangesAsync();
        }

        /// <summary>
        /// Delete proctoringPictures by userExamSession Id
        /// </summary>
        /// <param name="userExamSessionId"></param>
        public async Task DeleteFromDBProctoringPicturesAsync(int userExamSessionId, string userDelete)
        {
            var proctoringImages = await GetProctoringPicturesEntityAsync(userExamSessionId);

            foreach (var proctoringImage in proctoringImages)
            {
                proctoringImage.DateDelete = DateTime.Now;
                proctoringImage.UserDelete = userDelete;
            }

            await _proctoringContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProctoringImageForUserExamDto>> GetProctoringImagesPaginatedByUserExam(int userExamSessionId, int pageSize, int pageIndex)
        {
            // Check page size and page index
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10000;

            var proctoringImagesForUserExam = await GetProctoringImagesByUserExamIdQuery(userExamSessionId)
                .Select(pi => new ProctoringImageForUserExamDto 
                {
                    ImageByte = File.ReadAllBytes(pi.ContainerImageId),
                    ImageTimeStamp = pi.ImageTimeStamp
                })
                .OrderBy(pi => pi.ImageTimeStamp)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return proctoringImagesForUserExam;
        }

        public async Task<int> GetCountProctoringImagesPaginatedByUserExamAsync(int userExamSessionId)
        {
            var proctoringImagesQuery = GetProctoringImagesByUserExamIdQuery(userExamSessionId);
            return await proctoringImagesQuery.CountAsync();
        }

        /// <summary>
        /// GetProctoringImagesByUserExamIdQuery
        /// </summary>
        /// <param name="userExamSessionId">userExamSession Id.</param>
        /// <returns>Prepared query with specified conditions, which can be used further to perform required selections.</returns>
        private IQueryable<ProctoringImage> GetProctoringImagesByUserExamIdQuery(int userExamSessionId)
        {
            var examsSessionsQuery = _proctoringContext.ProctoringImages
                .AsNoTracking()
                .Where(pi => pi.UserExamSessionId == userExamSessionId)
                .Where(IsntDeletedProctoringImage);

            return examsSessionsQuery;
        }

        private readonly Expression<Func<ProctoringImage, bool>> IsntDeletedProctoringImage = (proctoringImage) => (proctoringImage.DateDelete == null || proctoringImage.DateDelete > DateTime.Now);
        private readonly Expression<Func<UserImageCheck, bool>> IsntDeletedUserCheckImage = (imageCheck) => (imageCheck.DateDelete == null || imageCheck.DateDelete > DateTime.Now);
    }
}

