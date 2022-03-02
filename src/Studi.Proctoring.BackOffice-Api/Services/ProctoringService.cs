using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public class ProctoringService : IProctoringService
    {
        private readonly IProctoringRepository proctoringRepository;

        public ProctoringService(IServiceProvider serviceProvider)
        {
            proctoringRepository = serviceProvider.ProctoringRepository();
        }

        public async Task<ProctoringImageForUserExamPageDto> GetProctoringImagesPaginatedByUserExam(int userExamSessionId, int pageSize, int pageIndex)
        {
            var proctoringImagesForUserExamCount = await proctoringRepository.GetCountProctoringImagesPaginatedByUserExamAsync(userExamSessionId);
            var proctoringImagesForUserExamDtos = await proctoringRepository.GetProctoringImagesPaginatedByUserExam(userExamSessionId, pageSize, pageIndex);

            return new ProctoringImageForUserExamPageDto
            {
                PageIndex = pageIndex,
                TotalItemsCount = proctoringImagesForUserExamCount,
                ProctoringImagesPage = proctoringImagesForUserExamDtos
            };
        }  
    }
}
