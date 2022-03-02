using Studi.Proctoring.BackOffice_Api.Repositories;

using System;
using Microsoft.Extensions.DependencyInjection;
using Studi.Proctoring.BackOffice_Api.Repositories.Interfaces;

namespace Studi.Proctoring.BackOffice_Api.Helpers
{
    // Get all Repositories from service provider (methods extensions)
    public static class RepositoryProviderExtensions
    {
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

        /// <summary>
        /// The method returns the reference to the IPasswordService instance.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IPasswordService PasswordService(this IServiceProvider services)
        {
            return services.GetService<IPasswordService>();
        }

        /// <summary>
        /// GetIAdminUserRepository.
        /// </summary>
        /// <returns><see cref="IAdminUserRepository"/>.</returns>
        public static IAdminUserRepository AdminUserRepository(this IServiceProvider services)
        {
            return services.GetService<IAdminUserRepository>();
        }

        /// <summary>
        /// GetIUserRepository.
        /// </summary>
        /// <returns><see cref="IUserRepository"/>.</returns>
        public static IUserRepository UserRepository(this IServiceProvider services)
        {
            return services.GetService<IUserRepository>();
        }

        public static IGlobalSettingRepository GlobalSettingRepository(this IServiceProvider services)
        {
            return services.GetService<IGlobalSettingRepository>();
        }
    }
}
