using Microsoft.EntityFrameworkCore.Migrations;

namespace emojipad.Migrations
{
    public partial class RemoveFolders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Emojis",
                table: "Emojis");

            migrationBuilder.DropColumn(
                name: "RelativeFilePath",
                table: "Emojis");

            migrationBuilder.DropColumn(
                name: "FolderName",
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emojis",
                table: "Emojis",
                column: "FileName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Emojis",
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

            migrationBuilder.AddColumn<string>(
                name: "FolderName",
                table: "Emojis",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emojis",
                table: "Emojis",
                column: "RelativeFilePath");
        }
    }
}
