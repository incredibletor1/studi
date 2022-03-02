using Studi.Api.Proctoring.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Repositories
{
    public interface IProctoringRepository
    {
        Task CreateProctoringImageAsync(ProctoringImageDto proctoringImage);
        Task CreateUserCheckImageAsync(UserImageCheckDto userImageCheck);
        Task<ProctoringImageDto> GetLastProctoringImageOfUserExamSessionAsync(int userExamSessionId);
    }
}
