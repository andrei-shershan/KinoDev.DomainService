using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KinoDev.DomainService.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AdjustOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashCode",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "EmailSent",
                table: "Orders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailSent",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "HashCode",
                table: "Orders",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
