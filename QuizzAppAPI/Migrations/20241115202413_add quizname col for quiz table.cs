using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizzAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class addquiznamecolforquiztable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuizName",
                table: "Quizzes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuizName",
                table: "Quizzes");
        }
    }
}
