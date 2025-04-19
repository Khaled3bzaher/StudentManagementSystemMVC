using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementMVCProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class courseimprovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_HeadTeacherID",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Prerequisites",
                table: "Courses");

            migrationBuilder.AddColumn<int>(
                name: "PrerequisiteCourseId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadTeacherID",
                table: "Departments",
                column: "HeadTeacherID");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_PrerequisiteCourseId",
                table: "Courses",
                column: "PrerequisiteCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Courses_PrerequisiteCourseId",
                table: "Courses",
                column: "PrerequisiteCourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Courses_PrerequisiteCourseId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Departments_HeadTeacherID",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Courses_PrerequisiteCourseId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "PrerequisiteCourseId",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "Prerequisites",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadTeacherID",
                table: "Departments",
                column: "HeadTeacherID",
                unique: true,
                filter: "[HeadTeacherID] IS NOT NULL");
        }
    }
}
