using Studi.Api.Proctoring.Helpers;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Api.Proctoring.Repositories;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Services
{
    public class GlobalSettingService : IGlobalSettingService
    {
        private readonly IGlobalSettingRepository _globalSettingRepository;

        public GlobalSettingService(IServiceProvider serviceProvider)
        {
            this._globalSettingRepository = serviceProvider.GlobalSettingRepository();
        }

        #region Get operations
        
        public async Task<GlobalSettingDto> GetProctoringImageIntervalAsync()
        {
            return await _globalSettingRepository.GetGlobalSettingByCodeAsync(GlobalSettingEnum.ProctoringImageInterval);
        }

        public async Task<GlobalSettingDto> GetMaxCheckImageSizeAsync()
        {
            return await _globalSettingRepository.GetGlobalSettingByCodeAsync(GlobalSettingEnum.MaxCheckImageSize);
        }

        public async Task<GlobalSettingDto> GetMaxProctoringImageSizeAsync()
        {
            return await _globalSettingRepository.GetGlobalSettingByCodeAsync(GlobalSettingEnum.MaxProctoringImageSize);
        }

        #endregion
    }
}
