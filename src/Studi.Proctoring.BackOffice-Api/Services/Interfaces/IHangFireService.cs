using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services.Interfaces
{
    public interface IHangFireService
    {
        Task<List<int>> DeleteAllObsoleteExamSessionsAndUserExamSessions();
        Task<List<int>> UpdateWronglyOngoingUserExamsToFinishWithErrorStatusAsync();
    }
}
