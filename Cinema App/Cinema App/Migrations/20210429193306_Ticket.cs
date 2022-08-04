using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinema_App.Migrations
{
    public partial class Ticket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "DateTime", "Describtion", "Duration", "Name", "PhotoPath" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, "Avengers", null });
        }
    }
}
