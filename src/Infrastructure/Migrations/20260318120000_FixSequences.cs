using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSequences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SELECT setval(pg_get_serial_sequence('cheeps', 'cheepid'), COALESCE(MAX(cheepid), 1)) FROM cheeps;
                SELECT setval(pg_get_serial_sequence('authors', 'authorid'), COALESCE(MAX(authorid), 1)) FROM authors;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
