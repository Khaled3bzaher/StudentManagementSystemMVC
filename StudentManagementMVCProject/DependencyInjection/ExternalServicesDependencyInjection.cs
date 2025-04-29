using Microsoft.AspNetCore.Identity.UI.Services;
using StudentManagementMVCProject.ExternalServices;

namespace StudentManagementMVCProject.DependencyInjection
{
    public static class ExternalServicesDependencyInjection
    {
        public static IServiceCollection AddExternalServicesDependencyInjection(this IServiceCollection services)
        {
            //Confirmation Mail
            services.AddSingleton<IEmailSender, EmailSenderService>();

            return services;
        }
    }
}
