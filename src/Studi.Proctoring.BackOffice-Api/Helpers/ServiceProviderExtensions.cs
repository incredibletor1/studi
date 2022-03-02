using Studi.Proctoring.BackOffice_Api.Repositories;
using Studi.Proctoring.BackOffice_Api.Services;

using System;
using Microsoft.Extensions.DependencyInjection;
using Studi.Proctoring.BackOffice_Api.Services.Interfaces;

namespace Studi.Proctoring.BackOffice_Api.Helpers
{
    // Get all Services from service provider (methods extensions)
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// GetTokenService.
        /// </summary>
        /// <returns><see cref="ITokenService"/>.</returns>
        public static ITokenService TokenService(this IServiceProvider services)
        {
            return services.GetService<ITokenService>();
        }

        /// <summary>
        /// GetAdminTokenService.
        /// </summary>
        /// <param name="services"></param>
        /// <returns><see cref="IAdminTokenService"/>.</returns>
        public static IAdminTokenService AdminTokenService(this IServiceProvider services)
        {
            return services.GetService<IAdminTokenService>();
        }

        /// <summary>
        /// GetAdminUserService.
        /// </summary>
        /// <param name="services"></param>
        /// <returns><see cref="IAdminUserService"/>.</returns>
        public static IAdminUserService AdminUserService(this IServiceProvider services)
        {
            return services.GetService<IAdminUserService>();
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
        /// GetImageService.
        /// </summary>
        /// <returns><see cref="IImageService"/>.</returns>
        public static IImageService ImageService(this IServiceProvider services)
        {
            return services.GetService<IImageService>();
        }

        /// <summary>
        /// GetHangFireService.
        /// </summary>
        /// <returns><see cref="IHangFireService"/>.</returns>
        public static IHangFireService HangFireService(this IServiceProvider services)
        {
            return services.GetService<IHangFireService>();
        }

        /// <summary>
        /// GetArchivationService.
        /// </summary>
        /// <returns><see cref="IArchivationService"/>.</returns>
        public static IArchivationService ArchivationService(this IServiceProvider services)
        {
            return services.GetService<IArchivationService>();
        }

        /// <summary>
        /// GetUserService.
        /// </summary>
        /// <returns><see cref="IUserService"/>.</returns>
        public static IUserService UserService(this IServiceProvider services)
        {
            return services.GetService<IUserService>();
        }

        /// <summary>
        /// GetProctoringService.
        /// </summary>
        /// <returns><see cref="IProctoringService"/>.</returns>
        public static IProctoringService ProctoringService(this IServiceProvider services)
        {
            return services.GetService<IProctoringService>();
        }
        
        public static IGlobalSettingService GlobalSettingService(this IServiceProvider services)
        {
            return services.GetService<IGlobalSettingService>();
        }
    }
}
