using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcProject.Migrations
{
    /// <inheritdoc />
    public partial class changeapp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Applications");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Applications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_CourseId",
                table: "Applications",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Courses_CourseId",
                table: "Applications",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Courses_CourseId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_CourseId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
