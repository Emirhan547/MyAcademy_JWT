using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAcademy_JWT.Migrations
{
    /// <inheritdoc />
    public partial class mig_AddPlayedAtToUserSongHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayedAtUtc",
                table: "UserSongHistories",
                newName: "PlayedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PackageId",
                table: "AspNetUsers",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Packages_PackageId",
                table: "AspNetUsers",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Packages_PackageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PackageId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PlayedAt",
                table: "UserSongHistories",
                newName: "PlayedAtUtc");
        }
    }
}
