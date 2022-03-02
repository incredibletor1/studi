using Microsoft.AspNetCore.Http;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Services
{
    public interface IImageService
    {
        byte[] ConvertImageToByte(IFormFile image);
        int GetContainerType(ContainerTypeEnum containerType);
        Task CreateUserStartExamImageAsync(UserExamSessionDto userExamSession, IFormFile photo, ImageTypeEnum userImage);
        Task CreateUserProctoringImageAsync(int userId, int userExamSessionId, IFormFile imageFile);
    }
}
