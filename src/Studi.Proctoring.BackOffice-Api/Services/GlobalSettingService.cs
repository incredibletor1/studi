using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Studi.Proctoring.BackOffice_Api.Exceptions;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Repositories;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public class GlobalSettingService : IGlobalSettingService
    {
        private readonly IGlobalSettingRepository _globalSettingRepository;

        public GlobalSettingService(IServiceProvider serviceProvider)
        {
            this._globalSettingRepository = serviceProvider.GlobalSettingRepository();
        }

        #region Get operations

        public async Task<IEnumerable<GlobalSettingDto>> GetAllGlobalSettingsAsync()
        {
            var settings = await _globalSettingRepository.GetAllGlobalSettingsAsync();
            return settings;
        }

        public async Task<GlobalSettingDto> GetGlobalSettingByCodeAsync(GlobalSettingEnum globalSettingCode)
        {
            return (await _globalSettingRepository.GetGlobalSettingByCodeAsync(globalSettingCode));
        }

        public async Task<GlobalSettingDto> GetMaxCheckImageSizeAsync()
        {
            return await _globalSettingRepository.GetMaxCheckImageSizeAsync();
        }

        public async Task<GlobalSettingDto> GetMaxProctoringImageSizeAsync()
        {
            return await _globalSettingRepository.GetMaxProctoringImageSizeAsync();
        }

        public async Task<GlobalSettingDto> GetPasswordValidityDurationAsync()
        {
            return await _globalSettingRepository.GetPasswordValidityDurationAsync();
        }

        public async Task<GlobalSettingDto> GetProctoringImageIntervalAsync()
        {
            return await _globalSettingRepository.GetProctoringImageIntervalAsync();
        }

        #endregion

        #region Update operations

        public async Task<GlobalSettingDto> UpdateGlobalSettingByCodeAsync(GlobalSettingEnum globalSettingCode, dynamic newValue, string userUpdate)
        {
            return await _globalSettingRepository.UpdateGlobalSettingByCodeAsync(globalSettingCode, newValue, userUpdate);
        }

        public async Task<GlobalSettingDto> UpdateValueOfMaxCheckImageSizeAsync(dynamic newValue, string userUpdate)
        {
            return await _globalSettingRepository.UpdateValueOfMaxCheckImageSizeAsync(newValue, userUpdate);
        }

        public async Task<GlobalSettingDto> UpdateValueOfMaxProctoringImageSizeAsync(dynamic newValue, string userUpdate)
        {
            return await _globalSettingRepository.UpdateValueOfMaxProctoringImageSizeAsync(newValue, userUpdate);
        }

        public async Task<GlobalSettingDto> UpdateValueOfPasswordValidityDurationAsync(dynamic newValue, string userUpdate)
        {
            return await _globalSettingRepository.UpdateValueOfPasswordValidityDurationAsync(newValue, userUpdate);
        }

        public async Task<GlobalSettingDto> UpdateValueOfProctoringImageIntervalAsync(dynamic newValue, string userUpdate)
        {
            if ((newValue as int?).Value < 0)
                throw new GlobalSettingWrongValueTypeException("Proctoring image interval cannot be negative.");

            return await _globalSettingRepository.UpdateValueOfProctoringImageIntervalAsync(newValue, userUpdate);
        }

        #endregion
    }
}
