using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Studi.Api.Proctoring.Helpers;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Repositories
{
    public class GlobalSettingRepository : IGlobalSettingRepository
    {
        private readonly IProctoringContext _proctoringContext;

        public GlobalSettingRepository(IProctoringContext proctoringContext, IMapper mapper)
        {
            this._proctoringContext = proctoringContext;
            ConversionExtension.InitMapper(mapper);
        }

        #region Get operations

        public async Task<GlobalSettingDto> GetGlobalSettingByCodeAsync(GlobalSettingEnum globalSettingCode)
        {
            return (await GetGlobalSettingEntityByCodeAsync(globalSettingCode)).ToDto();
        }

        #endregion

        #region private methods

        private async Task<GlobalSetting> GetGlobalSettingEntityByCodeAsync(GlobalSettingEnum globalSettingCode)
        {
            var setting = await _proctoringContext.GlobalSettings.Include(setting => setting.data_DataType).AsNoTracking()
                            .Where(setting => setting.Code == CodeToString(globalSettingCode))
                            .Where(IsntDeleted)
                            .FirstOrDefaultAsync();
            return setting;
        }

        #endregion

        private string CodeToString(GlobalSettingEnum code)
        {
            switch (code)
            {
                case GlobalSettingEnum.ProctoringImageInterval:
                    return "PROCTORING_IMAGE_INTERVAL";
                case GlobalSettingEnum.MaxProctoringImageSize:
                    return "MAX_PROCTORING_IMAGE_SIZE";
                case GlobalSettingEnum.MaxCheckImageSize:
                    return "MAX_CHECK_IMAGE_SIZE";
                case GlobalSettingEnum.PasswordValidityDuration:
                    return "PASSWORD_VALIDITY_DURATION";
            }

            return code.ToString();
        }

        private readonly Expression<Func<GlobalSetting, bool>> IsntDeleted = (user) => (user.DateDelete == null || user.DateDelete > DateTime.Now);
    }
}
