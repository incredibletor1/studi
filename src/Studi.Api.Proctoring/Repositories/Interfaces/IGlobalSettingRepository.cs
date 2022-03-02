using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Repositories
{
    public interface IGlobalSettingRepository
    {
        Task<GlobalSettingDto> GetGlobalSettingByCodeAsync(GlobalSettingEnum globalSettingCode);
    }
}
