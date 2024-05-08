using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BisleriumServer.Migrations
{
    /// <inheritdoc />
    public partial class likecountadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "likesCount",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "likesCount",
                table: "Blogs");
        }
    }
}
