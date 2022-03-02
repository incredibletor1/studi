using Microsoft.Extensions.Caching.Memory;
using Studi.Api.Proctoring.Helpers;
using Studi.Api.Proctoring.Repositories;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Services
{
    public class CacheService : ICacheService
    {
        private IMemoryCache _cache;
        private MemoryCacheEntryOptions cacheOptions;
        private readonly IUserService userService;
        private readonly IUserExamSessionService userExamSessionService;
        private readonly IProctoringRepository proctoringRepository;

        // TODO: this value have to be stored in the config file
        private const int cacheSlidingTimeoutInMinutes = 5;

        public CacheService(IMemoryCache memoryCache, IServiceProvider serviceProvider)
        {
            this._cache = memoryCache;
            userService = serviceProvider.UserService();
            userExamSessionService = serviceProvider.UserExamSessionService();
            proctoringRepository = serviceProvider.ProctoringRepository();

            // Initialize cache options with sliding expiration delay
            cacheOptions = new MemoryCacheEntryOptions();
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(cacheSlidingTimeoutInMinutes));
        }

        public async Task<int> GetUserIdByLmsUserIdAsync(int lmsUserId)
        {
            Func<string[], Task<int>> dbGetMethod = async (param) => (await userService.GetUserByLmsUserIdAsync(Convert.ToInt32(param[0]))).Id;
            
            return await GetIdFromCacheByKey("userIdByLmsId", dbGetMethod, lmsUserId.ToString());
        }

        public void RemoveFromCacheUserIdByLmsUserId(int lmsUserId)
        {
            _cache.Remove(BuiltCacheKey("userIdByLmsId", lmsUserId.ToString()));
        }

        public async Task<int> GetUserExamSessionIdIfOngoingAsync(int lmsUserId, string evaluationCode, string promotionCode, string filiereCode)
        {
            Func<string[], Task<int>> getUserExamSessionAsync = async (param) => 
            {
                var userExam = await userExamSessionService.GetUserExamSessionAsync(Convert.ToInt32(param[0]), param[1], param[2], param[3]);
                
                // Check for exam status
                if (userExam.StatusId != (int)ExamStatusEnum.Ongoing)
                    throw new ArgumentException($"User's exam has a 'finished' status, therefore the current operation cannot be performed anymore on userExamId={userExam.Id}");
                
                return userExam.Id;
            };

            return await GetIdFromCacheByKey("userExamId", getUserExamSessionAsync, lmsUserId.ToString(), evaluationCode, promotionCode, filiereCode);
        }

        public void RemoveFromCacheUserExamSessionId(int lmsUserId, string evaluationCode, string promotionCode, string filiereCode)
        {
            _cache.Remove(BuiltCacheKey("", lmsUserId.ToString(), evaluationCode, promotionCode, filiereCode));
        }

        public async Task<int> GetImageMaxOrderByUserExamId(int userExamSessionId)
        {
            Func<string[], Task<int>> dbGetMethod = async (param) => 
                (await proctoringRepository.GetLastProctoringImageOfUserExamSessionAsync(Convert.ToInt32(param[0])))?.Order ?? 0;

            return await GetIdFromCacheByKey("imageMaxOrder", dbGetMethod, userExamSessionId.ToString());
        }

        public async Task UpdateCacheImageMaxOrderByUserExamId(int userExamSessionId, int nextOrder)
        {
            // Build key from params
            var key = BuiltCacheKey("imageMaxOrder", userExamSessionId.ToString());
            _cache.Set(key, nextOrder, cacheOptions);
            return;
        }

        public void RemoveFromCacheImageMaxOrderByUserExamId(int userExamSessionId)
        {
            _cache.Remove(BuiltCacheKey("imageMaxOrder", userExamSessionId.ToString()));
        }

        public async Task<DateTime> GetLastImageTimestampByUserExamId(int userExamSessionId)
        {
            // Method to retrieve timestamp of the latest proctoring image from database
            Func<string[], Task<DateTime>> dbGetMethod = async (param) =>
                ((await proctoringRepository
                .GetLastProctoringImageOfUserExamSessionAsync(Convert.ToInt32(param[0])))?
                .ImageTimeStamp ?? DateTime.Now.AddDays(-1));

            return await GetFromCacheByKey<DateTime>("lastImageTime", dbGetMethod, userExamSessionId.ToString());
        }

        public async Task UpdateCacheLastImageTimestampByUserExamId(int userExamSessionId, DateTime imageTimestamp)
        {
            // Build key from params
            var key = BuiltCacheKey("lastImageTime", userExamSessionId.ToString());
            _cache.Set<DateTime>(key, imageTimestamp, cacheOptions);
            return;
        }

        /// <summary>
        /// Retrieve an Id (or any int) from: keyName + params in the cache, else from the provided func + add to cache
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="methodToGetFromDB"></param>
        /// <param name="keyParams"></param>
        /// <returns></returns>
        private async Task<int> GetIdFromCacheByKey(string keyName, Func<string[], Task<int>> methodToGetFromDB, params string[] keyParams)
        {
            return await GetFromCacheByKey<int>(keyName, methodToGetFromDB, keyParams);
        }

        private async Task<T> GetFromCacheByKey<T>(string keyName, Func<string[], Task<T>> methodToGetFromDB, params string[] keyParams)
        {
            // Build cache key from params
            var key = BuiltCacheKey(keyName, keyParams);

            // Look for the key in cache
            T valueFromCache;
            if (!_cache.TryGetValue<T>(key, out valueFromCache))
            {
                // Key not found in cache: get data from database & add it to cache
                valueFromCache = await methodToGetFromDB(keyParams);
                _cache.Set(key, valueFromCache, cacheOptions);
            }

            return valueFromCache;
        }

        /// <summary>
        /// Build key from params
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="keyParams"></param>
        /// <returns></returns>
        private string BuiltCacheKey(string keyName, params string[] keyParams)
        {
            var key = keyName + ">" + string.Join(',', keyParams);
            return key;
        }
    }
}
