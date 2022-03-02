using Studi.Api.Proctoring.Repositories;
using Studi.Api.Proctoring.Services;

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Studi.Api.Proctoring.Helpers
{
    // Get all Services from service provider (methods extensions)
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// GetCacheService
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ICacheService CacheService(this IServiceProvider services)
        {
            return services.GetService<ICacheService>();
        }

        /// <summary>
        /// GetTokenService.
        /// </summary>
        /// <returns><see cref="ITokenService"/>.</returns>
        public static ITokenService TokenService(this IServiceProvider services)
        {
            return services.GetService<ITokenService>();
        }

        /// <summary>
        /// GetUserService.
        /// </summary>
        /// <returns><see cref="IUserRepository"/>.</returns>
        public static IUserService UserService(this IServiceProvider services)
        {
            return services.GetService<IUserService>();
        }

        /// <summary>
        /// GetExamSessionService.
        /// </summary>
        /// <returns><see cref="IExamSessionService"/>.</returns>
        public static IExamSessionService ExamSessionService(this IServiceProvider services)
        {
            return services.GetService<IExamSessionService>();
        }

        /// <summary>
        /// GetUserExamSessionService.
        /// </summary>
        /// <returns><see cref="IUserExamSessionService"/>.</returns>
        public static IUserExamSessionService UserExamSessionService(this IServiceProvider services)
        {
            return services.GetService<IUserExamSessionService>();
        }

        /// <summary>
        /// GetLmsApiClientService.
        /// </summary>
        /// <returns><see cref="ILmsApiClient"/>.</returns>
        public static ILmsApiClient LmsApiClientService(this IServiceProvider services)
        {
            return services.GetService<ILmsApiClient>();
        }

        /// <summary>
        /// GetProctoringService.
        /// </summary>
        /// <returns><see cref="IProctoringService"/>.</returns>
        public static IProctoringService ProctoringService(this IServiceProvider services)
        {
            return services.GetService<IProctoringService>();
        }

        /// <summary>
        /// GetImageService.
        /// </summary>
        /// <returns><see cref="IImageService"/>.</returns>
        public static IImageService ImageService(this IServiceProvider services)
        {
            return services.GetService<IImageService>();
        }

        public static IGlobalSettingService GlobalSettingService(this IServiceProvider services)
        {
            return services.GetService<IGlobalSettingService>();
        }
    }
}
