using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParParWebsite.Api.Migrations
{
    /// <inheritdoc />
    public partial class changedcollectiontoportfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectionImages_Collections_CollectionId",
                table: "CollectionImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Collections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostImages",
                table: "CollectionImages");

            migrationBuilder.RenameTable(
                name: "Collections",
                newName: "Portfolios");

            migrationBuilder.RenameTable(
                name: "CollectionImages",
                newName: "PortfolioImages");

            migrationBuilder.RenameIndex(
                name: "IX_PostImages_PostId",
                table: "PortfolioImages",
                newName: "IX_PortfolioImages_PortfolioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Portfolios",
                table: "Portfolios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PortfolioImages",
                table: "PortfolioImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PortfolioImages_PortfolioId",
                table: "PortfolioImages",
                column: "CollectionId",
                principalTable: "Portfolios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortfolioImages_Portfolios_CollectionId",
                table: "PortfolioImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Portfolios",
                table: "Portfolios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PortfolioImages",
                table: "PortfolioImages");

            migrationBuilder.RenameTable(
                name: "Portfolios",
                newName: "Collections");

            migrationBuilder.RenameTable(
                name: "PortfolioImages",
                newName: "CollectionImages");

            migrationBuilder.RenameIndex(
                name: "IX_PortfolioImages_CollectionId",
                table: "CollectionImages",
                newName: "IX_PostImages_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Collections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostImages",
                table: "CollectionImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionImages_Collections_CollectionId",
                table: "CollectionImages",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
