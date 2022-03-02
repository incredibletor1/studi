using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.Database.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public interface IGlobalSettingRepository
    {
        Task<IEnumerable<GlobalSettingDto>> GetAllGlobalSettingsAsync();
        Task<GlobalSettingDto> GetGlobalSettingByCodeAsync(GlobalSettingEnum globalSettingCode);
        Task<GlobalSettingDto> GetMaxCheckImageSizeAsync();
        Task<GlobalSettingDto> GetMaxProctoringImageSizeAsync();
        Task<GlobalSettingDto> GetPasswordValidityDurationAsync();
        Task<GlobalSettingDto> GetProctoringImageIntervalAsync();
        Task<GlobalSettingDto> UpdateGlobalSettingByCodeAsync(GlobalSettingEnum globalSettingCode, dynamic newValue, string userUpdate);
        Task<GlobalSettingDto> UpdateValueOfMaxCheckImageSizeAsync(dynamic newValue, string userUpdate);
        Task<GlobalSettingDto> UpdateValueOfMaxProctoringImageSizeAsync(dynamic newValue, string userUpdate);
        Task<GlobalSettingDto> UpdateValueOfPasswordValidityDurationAsync(dynamic newValue, string userUpdate);
        Task<GlobalSettingDto> UpdateValueOfProctoringImageIntervalAsync(dynamic newValue, string userUpdate);
    }
}