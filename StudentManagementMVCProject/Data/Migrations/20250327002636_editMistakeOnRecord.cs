using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementMVCProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class editMistakeOnRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicRecords_Students_UserId",
                table: "AcademicRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_AcademicRecords_UserId",
                table: "AcademicRecords");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AcademicRecords");

            migrationBuilder.RenameColumn(
                name: "EndDatte",
                table: "Semesters",
                newName: "EndDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CourseRegistrationEndDate",
                table: "Semesters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CourseRegistrationStartDate",
                table: "Semesters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "AcademicRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicRecords_StudentId",
                table: "AcademicRecords",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicRecords_Students_StudentId",
                table: "AcademicRecords",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicRecords_Students_StudentId",
                table: "AcademicRecords");

            migrationBuilder.DropIndex(
                name: "IX_AcademicRecords_StudentId",
                table: "AcademicRecords");

            migrationBuilder.DropColumn(
                name: "CourseRegistrationEndDate",
                table: "Semesters");

            migrationBuilder.DropColumn(
                name: "CourseRegistrationStartDate",
                table: "Semesters");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Semesters",
                newName: "EndDatte");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "AcademicRecords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "AcademicRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicRecords_UserId",
                table: "AcademicRecords",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicRecords_Students_UserId",
                table: "AcademicRecords",
                column: "UserId",
                principalTable: "Students",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
