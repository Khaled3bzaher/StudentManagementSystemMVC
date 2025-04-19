using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementMVCProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class improvementStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "Users",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "AcademicLevel",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AcademicLevel",
                table: "Students");
        }
    }
}
