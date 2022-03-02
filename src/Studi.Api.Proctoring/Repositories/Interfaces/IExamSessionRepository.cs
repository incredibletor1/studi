using Studi.Api.Proctoring.Models.DTO;
using Studi.Api.Proctoring.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Repositories
{
    public interface IExamSessionRepository
    {
        Task<ExamSessionDto> GetExamSessionByIdAsync(int id);
        Task<ExamSessionDto> GetExamSessionAsync(string evaluationCode, string promotionCode, string filiereCode);
        Task<ExamSessionDto> CreateExamSessionAsync(ExamSessionDto examSession);
    }
}
