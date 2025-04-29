namespace StudentManagementMVCProject.DependencyInjection
{
    public static class ExternalLoginsDependencyInjection
    {
        public static IServiceCollection AddExternalLoginsDependencyInjection(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            services.AddAuthentication().AddGoogle(
                options =>
                {
                    options.ClientId = configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                    options.CallbackPath = "/signin-google";
                });
            return services;
        }
    }
}
