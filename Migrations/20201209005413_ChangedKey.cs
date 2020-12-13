using Microsoft.EntityFrameworkCore.Migrations;

namespace emojipad.Migrations
{
    public partial class ChangedKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Emojis",
                table: "Emojis");

            migrationBuilder.DropColumn(
                name: "FullFilePath",
                table: "Emojis");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Emojis",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "RelativeFilePath",
                table: "Emojis",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emojis",
                table: "Emojis",
                column: "RelativeFilePath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Emojis",
                table: "Emojis");

            migrationBuilder.DropColumn(
                name: "RelativeFilePath",
                table: "Emojis");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Emojis",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullFilePath",
                table: "Emojis",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emojis",
                table: "Emojis",
                column: "FileName");
        }
    }
}
