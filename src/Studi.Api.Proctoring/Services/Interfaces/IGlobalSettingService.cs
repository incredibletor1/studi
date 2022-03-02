using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Services
{
    public interface IGlobalSettingService
    {
        Task<GlobalSettingDto> GetProctoringImageIntervalAsync();
        Task<GlobalSettingDto> GetMaxCheckImageSizeAsync();
        Task<GlobalSettingDto> GetMaxProctoringImageSizeAsync();
    }
}
