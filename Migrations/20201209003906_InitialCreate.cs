using Microsoft.EntityFrameworkCore.Migrations;

namespace emojipad.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emojis",
                columns: table => new
                {
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FullFilePath = table.Column<string>(type: "TEXT", nullable: true),
                    UsedFrequency = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emojis", x => x.FileName);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emojis");
        }
    }
}
