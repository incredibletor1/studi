using System.Collections.Generic;
using System.Threading.Tasks;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Api.Proctoring.Models.VM;

namespace Studi.Api.Proctoring.Services
{
    public interface IProctoringService
    {
        Task<int> InitUserExamAndCreateIDCheckImagesAsync(StartExamVM startExamVM, string lmsApiBaseUrl, string authToken);
        Task UploadProctoringImageAsync(UploadImageVM uploadImage, int lmsUserId);
        Task FinishUserExamAsync(int lmsUserId, string evaluationCode, string promotionCode, string filiereCode);
        Task<int?> UserExamMaterialCheckAndStartUserExamAsync(UserExamMaterialCheckDto userExamMaterialCheckDto);
    }
}
