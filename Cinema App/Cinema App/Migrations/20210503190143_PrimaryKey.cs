using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinema_App.Migrations
{
    public partial class PrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieUser",
                table: "MovieUser");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MovieUser",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MovieUser",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieUser",
                table: "MovieUser",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieUser",
                table: "MovieUser");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MovieUser");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MovieUser",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieUser",
                table: "MovieUser",
                column: "UserId");
        }
    }
}
