using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movie_Ticket_Booking.Migrations
{
    /// <inheritdoc />
    public partial class FixMovieSubImagesTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieSubImage_Movies_MovieId",
                table: "MovieSubImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieSubImage",
                table: "MovieSubImage");

            migrationBuilder.RenameTable(
                name: "MovieSubImage",
                newName: "MovieSubImages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieSubImages",
                table: "MovieSubImages",
                columns: new[] { "MovieId", "Img" });

            migrationBuilder.CreateTable(
                name: "ActorMovie",
                columns: table => new
                {
                    ActorsId = table.Column<int>(type: "int", nullable: false),
                    MoviesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorMovie", x => new { x.ActorsId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_ActorMovie_Actors_ActorsId",
                        column: x => x.ActorsId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActorMovie_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorMovie_MoviesId",
                table: "ActorMovie",
                column: "MoviesId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieSubImages_Movies_MovieId",
                table: "MovieSubImages",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieSubImages_Movies_MovieId",
                table: "MovieSubImages");

            migrationBuilder.DropTable(
                name: "ActorMovie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieSubImages",
                table: "MovieSubImages");

            migrationBuilder.RenameTable(
                name: "MovieSubImages",
                newName: "MovieSubImage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieSubImage",
                table: "MovieSubImage",
                columns: new[] { "MovieId", "Img" });

            migrationBuilder.AddForeignKey(
                name: "FK_MovieSubImage_Movies_MovieId",
                table: "MovieSubImage",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
