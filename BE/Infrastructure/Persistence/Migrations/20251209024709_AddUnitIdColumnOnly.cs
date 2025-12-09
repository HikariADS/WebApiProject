using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiProject.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitIdColumnOnly : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'UnitId')
                BEGIN
                    ALTER TABLE [dbo].[AspNetUsers] DROP COLUMN [UnitId];
                END
            ");
        }
    }
}
