using System;
using System.Threading.Tasks;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.Database.Context;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public interface IImageRepository
    {
        void DeleteAllUserCheckImagesFilesByUserExam(int userId, int userExamId);
        void DeleteAllProctoringImagesFilesByUserExam(int userId, int userExamId);
    }
}
