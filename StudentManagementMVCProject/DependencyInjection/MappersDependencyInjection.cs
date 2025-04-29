using StudentManagementMVCProject.Mapping;

namespace StudentManagementMVCProject.DependencyInjection
{
    public static class MappersDependencyInjection
    {
        public static IServiceCollection AddMappersDependencyInjection(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AssemblyReference).Assembly);

            //services.AddAutoMapper(typeof(RoleProfile));
            //services.AddAutoMapper(typeof(CourseProfile));
            //services.AddAutoMapper(typeof(StudentProfile));
            //services.AddAutoMapper(typeof(DepartmentProfile));
            //services.AddAutoMapper(typeof(TeacherProfile));
            return services;
        }
    }
}
