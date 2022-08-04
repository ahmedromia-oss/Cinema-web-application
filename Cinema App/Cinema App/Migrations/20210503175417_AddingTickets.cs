using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinema_App.Migrations
{
    public partial class AddingTickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Movies_movieId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_movieId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Tickets",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "movieId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Tickets",
                table: "MovieUser",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tickets",
                table: "MovieUser");

            migrationBuilder.AddColumn<int>(
                name: "Tickets",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "movieId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_movieId",
                table: "AspNetUsers",
                column: "movieId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Movies_movieId",
                table: "AspNetUsers",
                column: "movieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
