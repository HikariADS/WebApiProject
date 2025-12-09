using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiProject.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add UnitId to AspNetUsers if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'UnitId')
                BEGIN
                    ALTER TABLE [dbo].[AspNetUsers] ADD [UnitId] NVARCHAR(MAX) NULL;
                END
            ");

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

            // Drop UnitId only if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'UnitId')
                BEGIN
                    ALTER TABLE [dbo].[AspNetUsers] DROP COLUMN [UnitId];
                END
            ");
        }
    }
}
