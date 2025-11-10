using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movie_Ticket_Booking.Migrations
{
    /// <inheritdoc />
    public partial class Edit_names_Cart_Promotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Movies_movieId",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "movieId",
                table: "Promotions",
                newName: "MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_Promotions_movieId",
                table: "Promotions",
                newName: "IX_Promotions_MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Movies_MovieId",
                table: "Promotions",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Movies_MovieId",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "MovieId",
                table: "Promotions",
                newName: "movieId");

            migrationBuilder.RenameIndex(
                name: "IX_Promotions_MovieId",
                table: "Promotions",
                newName: "IX_Promotions_movieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Movies_movieId",
                table: "Promotions",
                column: "movieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
