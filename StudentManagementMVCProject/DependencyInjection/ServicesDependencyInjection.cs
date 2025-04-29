using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Implementations;
using StudentManagementMVCProject.Repositories.Interfaces;

namespace StudentManagementMVCProject.DependencyInjection
{
    public static class ServicesDependencyInjection
    {
        public static IServiceCollection AddServicesDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IAcademicRecordService, AcademicRecordService>();
            services.AddScoped<ICourseRegistrationService, CourseRegistrationService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<ISemesterService, SemesterService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IRoleService, RoleService>();
            return services;
        }
    }
}
