using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ParParWebsite.Api.Migrations
{
    /// <inheritdoc />
    public partial class changedposttocollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename tables
            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Collections"
            );

            migrationBuilder.RenameTable(
                name: "PostImages",
                newName: "CollectionImages"
            );

            // Rename FK column if needed
            migrationBuilder.RenameColumn(
                name: "CollectionId",
                table: "CollectionImages",
                newName: "CollectionId"
            );

            // Update foreign key manually (drop and add if needed)
            migrationBuilder.DropForeignKey(
                name: "FK_PostImages_Posts_PostId",
                table: "CollectionImages"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionImages_Collections_CollectionId",
                table: "CollectionImages",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionImages");

            migrationBuilder.CreateTable(
                name: "PostImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostImages_Collections_PostId",
                        column: x => x.PostId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostImages_PostId",
                table: "PostImages",
                column: "CollectionId");
        }
    }
}
