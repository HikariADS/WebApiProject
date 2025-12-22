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

            // Add BelongToUnitId if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Storages]') AND name = 'BelongToUnitId')
                BEGIN
                    ALTER TABLE [dbo].[Storages] ADD [BelongToUnitId] NVARCHAR(MAX) NULL;
                END
            ");

            // Add ManagerId if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Storages]') AND name = 'ManagerId')
                BEGIN
                    ALTER TABLE [dbo].[Storages] ADD [ManagerId] NVARCHAR(MAX) NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop BelongToUnitId if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Storages]') AND name = 'BelongToUnitId')
                BEGIN
                    ALTER TABLE [dbo].[Storages] DROP COLUMN [BelongToUnitId];
                END
            ");

            // Drop ManagerId if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Storages]') AND name = 'ManagerId')
                BEGIN
                    ALTER TABLE [dbo].[Storages] DROP COLUMN [ManagerId];
                END
            ");

            // Drop UnitId if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'UnitId')
                BEGIN
                    ALTER TABLE [dbo].[AspNetUsers] DROP COLUMN [UnitId];
                END
            ");
        }
    }
}
