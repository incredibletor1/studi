using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public interface IImageService
    {
        Task DeleteAllImagesForUserExamsAsync(IEnumerable<UserExamSessionDto> userExamSessionsToDelete, string deleteUserName);
        Task<ExamSessionArchivedDto> GetExamSessionFileInformationByExamSessionIdAsync(int examSessionId);
    }
}
