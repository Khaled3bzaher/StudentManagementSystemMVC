using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagementMVCProject.Models;

namespace StudentManagementMVCProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<CourseSchedule> CourseSchedules { get; set; }
        public DbSet<AcademicRecord> AcademicRecords { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<Semester> Semesters { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");

            builder.Entity<User>()
                .Property(user => user.DateRegistered)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Student>()
                .Property(s => s.StudentNumber)
                .IsRequired(false);

            builder.Entity<User>()
                .HasIndex(user => user.NationalId)
                .IsUnique();

            builder.Entity<Teacher>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Teachers)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Department>()
                .HasOne(d=>d.HeadTeacher)
                .WithMany()
                .HasForeignKey(d=>d.HeadTeacherID)
                .OnDelete(DeleteBehavior.SetNull);

            

        }
    }
}
