using Microsoft.AspNetCore.Identity;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.Models;

namespace StudentManagementMVCProject.DependencyInjection
{
    public static class IdentityDependencyInjection
    {
        public static IServiceCollection AddIdentityDependencyInjection(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(
                options => {

                    options.SignIn.RequireConfirmedAccount = false ;
                    options.Tokens.AuthenticatorTokenProvider = null;

                }
            )
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            return services;
        }
    }
}
