using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiProject.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBelongToUnitIdAndManagerIdToStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BelongToUnitId",
                table: "Storages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "Storages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BelongToUnitId",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Storages");
        }
    }
}
