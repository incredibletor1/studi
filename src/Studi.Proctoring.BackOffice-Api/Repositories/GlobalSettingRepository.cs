using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Studi.Proctoring.BackOffice_Api.Exceptions;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
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

        public async Task<IEnumerable<GlobalSettingDto>> GetAllGlobalSettingsAsync()
        {
            var settings = (await GetAllGlobalSettingsEntitiesAsync())
                            .Select(setting => setting.ToDto());

            return settings;
        }

        public async Task<GlobalSettingDto> GetGlobalSettingByCodeAsync(GlobalSettingEnum globalSettingCode)
        {
            return (await GetGlobalSettingEntityByCodeAsync(globalSettingCode)).ToDto();
        }

        public async Task<GlobalSettingDto> GetMaxCheckImageSizeAsync()
        {
            return (await GetGlobalSettingEntityByCodeAsync(GlobalSettingEnum.MaxCheckImageSize)).ToDto();
        }

        public async Task<GlobalSettingDto> GetMaxProctoringImageSizeAsync()
        {
            return (await GetGlobalSettingEntityByCodeAsync(GlobalSettingEnum.MaxProctoringImageSize)).ToDto();
        }

        public async Task<GlobalSettingDto> GetPasswordValidityDurationAsync()
        {
            return (await GetGlobalSettingEntityByCodeAsync(GlobalSettingEnum.PasswordValidityDuration)).ToDto();
        }

        public async Task<GlobalSettingDto> GetProctoringImageIntervalAsync()
        {
            return (await GetGlobalSettingEntityByCodeAsync(GlobalSettingEnum.ProctoringImageInterval)).ToDto();
        }

        #endregion

        #region Update operations


        public async Task<GlobalSettingDto> UpdateGlobalSettingByCodeAsync(GlobalSettingEnum globalSettingCode, dynamic newValue, string userUpdate)
        {
            var setting = await GetWithTrackingGlobalSettingEntityByCodeAsync(globalSettingCode);

            if (setting is null)
                throw new NullReferenceException($"Unable to find global setting '{globalSettingCode.ToString()}' from database");

            Func<GlobalSettingEnum, DataTypeEnum, string> exceptionMessage = (globalSettingCode, dataTypeEnum) =>
                    $"Provided value for '{globalSettingCode.ToString()}' setting doesn't match the awaited datatype: {dataTypeEnum.ToString()}.";

            // Check whether new value type match awaited type
            // If the data type check succeed, update the value/oversizedValue
            switch (setting.DataTypeId)
            {
                case (int)DataTypeEnum.Bool:
                    if (newValue is bool)
                        setting.Value = (newValue as bool?).Value.ToString();
                    else
                        throw new GlobalSettingWrongValueTypeException(exceptionMessage(globalSettingCode, DataTypeEnum.Bool));
                    break;

                case (int)DataTypeEnum.Decimal:
                    if (newValue is decimal)
                        setting.Value = (newValue as decimal?).Value.ToString();
                    else
                        throw new GlobalSettingWrongValueTypeException(exceptionMessage(globalSettingCode, DataTypeEnum.Decimal));
                    break;

                case (int)DataTypeEnum.Int:
                    if (newValue is int)
                        setting.Value = (newValue as int?).Value.ToString();
                    else
                        throw new GlobalSettingWrongValueTypeException(exceptionMessage(globalSettingCode, DataTypeEnum.Int));
                    break;

                case (int)DataTypeEnum.LongText:
                    if (newValue is string)
                        setting.OversizedValue = (newValue as string).ToString();
                    else
                        throw new GlobalSettingWrongValueTypeException(exceptionMessage(globalSettingCode, DataTypeEnum.LongText));
                    break;

                case (int)DataTypeEnum.ShortText:
                    if (newValue is string)
                        setting.Value = (newValue as string).ToString();
                    else
                        throw new GlobalSettingWrongValueTypeException(exceptionMessage(globalSettingCode, DataTypeEnum.ShortText));
                    break;

                default:
                    throw new NotImplementedException($"DataType of Id {setting.DataTypeId} isn't handled by '{nameof(UpdateGlobalSettingByCodeAsync)}' method.");
            }

            // Update updation properties of the entity
            setting.DateUpdate = DateTime.Now;
            setting.UserUpdate = userUpdate;

            // Save the changes of entity to DB
            await _proctoringContext.SaveChangesAsync();

            return setting.ToDto();
        }

        public async Task<GlobalSettingDto> UpdateValueOfMaxCheckImageSizeAsync(dynamic newValue, string userUpdate)
        {
            return await UpdateGlobalSettingByCodeAsync(GlobalSettingEnum.MaxCheckImageSize, newValue, userUpdate);
        }

        public async Task<GlobalSettingDto> UpdateValueOfMaxProctoringImageSizeAsync(dynamic newValue, string userUpdate)
        {
            return await UpdateGlobalSettingByCodeAsync(GlobalSettingEnum.MaxProctoringImageSize, newValue, userUpdate);
        }

        public async Task<GlobalSettingDto> UpdateValueOfPasswordValidityDurationAsync(dynamic newValue, string userUpdate)
        {
            return await UpdateGlobalSettingByCodeAsync(GlobalSettingEnum.PasswordValidityDuration, newValue, userUpdate);
        }

        public async Task<GlobalSettingDto> UpdateValueOfProctoringImageIntervalAsync(dynamic newValue, string userUpdate)
        {
            return await UpdateGlobalSettingByCodeAsync(GlobalSettingEnum.ProctoringImageInterval, newValue, userUpdate);
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

        private async Task<GlobalSetting> GetWithTrackingGlobalSettingEntityByCodeAsync(GlobalSettingEnum globalSettingCode)
        {
            var setting = await _proctoringContext.GlobalSettings.Include(setting => setting.data_DataType)
                            .Where(setting => setting.Code == CodeToString(globalSettingCode))
                            .Where(IsntDeleted)
                            .FirstOrDefaultAsync();
            return setting;
        }

        private async Task<List<GlobalSetting>> GetAllGlobalSettingsEntitiesAsync()
        {
            var settings = await _proctoringContext.GlobalSettings
                            .Include(setting => setting.data_DataType)
                            .AsNoTracking()
                            .Where(IsntDeleted)
                            .ToListAsync();
            return settings;
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
