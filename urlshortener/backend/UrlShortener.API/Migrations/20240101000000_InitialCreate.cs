using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.API.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ShortenedUrls",
            columns: table => new
            {
                Id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Code = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                OriginalUrl = table.Column<string>(type: "TEXT", nullable: false),
                Title = table.Column<string>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                ClickCount = table.Column<long>(type: "INTEGER", nullable: false),
                LastAccessedIp = table.Column<string>(type: "TEXT", nullable: true),
                LastAccessedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ShortenedUrls", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ShortenedUrls_Code",
            table: "ShortenedUrls",
            column: "Code",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ShortenedUrls");
    }
}
