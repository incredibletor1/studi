using Studi.Api.Proctoring.Repositories;

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Studi.Api.Proctoring.Helpers
{
    // Get all Repositories from service provider (methods extensions)
    public static class RepositoryProviderExtensions
    {
        /// <summary>
        /// GetImageRepository.
        /// </summary>
        /// <returns><see cref="IImageRepository"/>.</returns>
        public static IUserRepository UserRepository(this IServiceProvider services)
        {
            return services.GetService<IUserRepository>();
        }

        /// <summary>
        /// GetProctoringRepository.
        /// </summary>
        /// <returns><see cref="IProctoringRepository"/>.</returns>
        public static IProctoringRepository ProctoringRepository(this IServiceProvider services)
        {
            return services.GetService<IProctoringRepository>();
        }

        /// <summary>
        /// GetImageRepository.
        /// </summary>
        /// <returns><see cref="IImageRepository"/>.</returns>
        public static IImageRepository ImageRepository(this IServiceProvider services)
        {
            return services.GetService<IImageRepository>();
        }

        /// <summary>
        /// GetExamSessionRepository.
        /// </summary>
        /// <returns><see cref="IExamSessionRepository"/>.</returns>
        public static IExamSessionRepository ExamSessionRepository(this IServiceProvider services)
        {
            return services.GetService<IExamSessionRepository>();
        }

        /// <summary>
        /// GetUserExamSessionRepository.
        /// </summary>
        /// <returns><see cref="IUserExamSessionRepository"/>.</returns>
        public static IUserExamSessionRepository UserExamSessionRepository(this IServiceProvider services)
        {
            return services.GetService<IUserExamSessionRepository>();
        }

        public static IGlobalSettingRepository GlobalSettingRepository(this IServiceProvider services)
        {
            return services.GetService<IGlobalSettingRepository>();
        }
    }
}
