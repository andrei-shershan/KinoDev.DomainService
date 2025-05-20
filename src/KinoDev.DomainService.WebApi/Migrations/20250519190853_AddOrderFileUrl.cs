using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KinoDev.DomainService.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderFileUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "Orders",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "Orders");
        }
    }
}
