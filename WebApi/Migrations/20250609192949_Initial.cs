using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    book_id = table.Column<string>(type: "TEXT", nullable: false),
                    best_book_id = table.Column<string>(type: "TEXT", nullable: false),
                    work_id = table.Column<string>(type: "TEXT", nullable: false),
                    books_count = table.Column<int>(type: "INTEGER", nullable: false),
                    isbn = table.Column<string>(type: "TEXT", nullable: false),
                    isbn13 = table.Column<string>(type: "TEXT", nullable: false),
                    authors = table.Column<string>(type: "TEXT", nullable: false),
                    original_publication_year = table.Column<float>(type: "REAL", nullable: true),
                    original_title = table.Column<string>(type: "TEXT", nullable: false),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    language_code = table.Column<string>(type: "TEXT", nullable: false),
                    average_rating = table.Column<decimal>(type: "TEXT", nullable: false),
                    ratings_count = table.Column<int>(type: "INTEGER", nullable: false),
                    work_ratings_count = table.Column<int>(type: "INTEGER", nullable: false),
                    work_text_reviews_count = table.Column<int>(type: "INTEGER", nullable: false),
                    ratings_1 = table.Column<int>(type: "INTEGER", nullable: false),
                    ratings_2 = table.Column<int>(type: "INTEGER", nullable: false),
                    ratings_3 = table.Column<int>(type: "INTEGER", nullable: false),
                    ratings_4 = table.Column<int>(type: "INTEGER", nullable: false),
                    ratings_5 = table.Column<int>(type: "INTEGER", nullable: false),
                    image_url = table.Column<string>(type: "TEXT", nullable: false),
                    small_image_url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
