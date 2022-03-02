using System;
using System.Threading.Tasks;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;

namespace Studi.Api.Proctoring.Repositories
{
    public interface IImageRepository
    {
        Task<string> CreateProctoringImageAsync(int userId, int userExamId, DateTime timestamp, byte[] imageData);
        Task<string> CreateUserCheckImageAsync(int userId, int userExamId, DateTime timestamp, ImageTypeEnum imageType, byte[] imageData);
    }
}
