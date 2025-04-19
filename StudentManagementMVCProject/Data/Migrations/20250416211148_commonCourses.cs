using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementMVCProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class commonCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCommon",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCommon",
                table: "Courses");
        }
    }
}
