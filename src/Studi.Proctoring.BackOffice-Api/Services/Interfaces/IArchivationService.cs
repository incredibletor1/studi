using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public interface IArchivationService
    {
        Task ArchivateExamSessionByIdAsync(int examSessionId, string userArchive);
        Task UnarchivateExamSessionByIdAsync(int examSessionId, string userUnarchive);
        Task UnarchivateExamSessionListAsync(List<int> examSessionIds, string userUnarchive);
    }
}
