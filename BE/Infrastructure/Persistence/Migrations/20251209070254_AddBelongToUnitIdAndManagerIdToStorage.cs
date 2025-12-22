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
            // Columns already added in UpdateAuth migration, so this is a no-op
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Columns already handled in UpdateAuth migration, so this is a no-op
        }
    }
}
