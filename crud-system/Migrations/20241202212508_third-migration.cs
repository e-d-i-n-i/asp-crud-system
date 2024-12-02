using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace crud_system.Migrations
{
    /// <inheritdoc />
    public partial class thirdmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Courses_CourseID",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_CourseID",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "CourseID",
                table: "Chapters");

            migrationBuilder.AlterColumn<string>(
                name: "CourseName",
                table: "Chapters",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_CourseName",
                table: "Chapters",
                column: "CourseName");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Courses_CourseName",
                table: "Chapters",
                column: "CourseName",
                principalTable: "Courses",
                principalColumn: "CourseName",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Courses_CourseName",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_CourseName",
                table: "Chapters");

            migrationBuilder.AlterColumn<string>(
                name: "CourseName",
                table: "Chapters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseID",
                table: "Chapters",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_CourseID",
                table: "Chapters",
                column: "CourseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Courses_CourseID",
                table: "Chapters",
                column: "CourseID",
                principalTable: "Courses",
                principalColumn: "CourseID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
