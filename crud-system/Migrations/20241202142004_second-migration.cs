using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace crud_system.Migrations
{
    /// <inheritdoc />
    public partial class secondmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Courses_CourseID",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_CourseID",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CourseID",
                table: "Students");

            migrationBuilder.AlterColumn<string>(
                name: "CourseName",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Courses_CourseName",
                table: "Courses",
                column: "CourseName");

            migrationBuilder.CreateIndex(
                name: "IX_Students_CourseName",
                table: "Students",
                column: "CourseName");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Courses_CourseName",
                table: "Students",
                column: "CourseName",
                principalTable: "Courses",
                principalColumn: "CourseName",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Courses_CourseName",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_CourseName",
                table: "Students");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Courses_CourseName",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "CourseName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseID",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Students_CourseID",
                table: "Students",
                column: "CourseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Courses_CourseID",
                table: "Students",
                column: "CourseID",
                principalTable: "Courses",
                principalColumn: "CourseID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
