using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentManagementMVCProject.Data;
using StudentManagementMVCProject.ExternalServices;
using StudentManagementMVCProject.Mapping;
using StudentManagementMVCProject.Models;
using StudentManagementMVCProject.Persistence.GenericRepository;
using StudentManagementMVCProject.Persistence.UnitOfWork;
using StudentManagementMVCProject.Repositories.Implementations;
using StudentManagementMVCProject.Repositories.Interfaces;

namespace StudentManagementMVCProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IAcademicRecordService, AcademicRecordService>();
            builder.Services.AddScoped<ICourseRegistrationService, CourseRegistrationService>();
            builder.Services.AddScoped<ITeacherService, TeacherService>();
            builder.Services.AddScoped<ISemesterService, SemesterService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddAutoMapper(typeof(RoleProfile));
            builder.Services.AddAutoMapper(typeof(CourseProfile));
            builder.Services.AddAutoMapper(typeof(StudentProfile));
            builder.Services.AddAutoMapper(typeof(DepartmentProfile));
            builder.Services.AddAutoMapper(typeof(TeacherProfile));
            //Cancel Confirmation Account
            builder.Services.AddIdentity<User, IdentityRole>(
                options => {

                    options.SignIn.RequireConfirmedAccount = false;
                    options.Tokens.AuthenticatorTokenProvider = null;

                }
            )
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
           
            builder.Services.AddControllersWithViews();
            //builder.Services.AddRazorPages();

            //Confirmation Mail
            builder.Services.AddTransient<IEmailSender, ConfirmationRegisterService>();
            var app = builder.Build();
           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Custom 404 handling
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == 404)
                {
                    context.HttpContext.Response.Redirect("/Home/NotFound");
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
