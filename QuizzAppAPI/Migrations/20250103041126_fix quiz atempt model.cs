using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizzAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class fixquizatemptmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswers",
                table: "QuizAttempts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalQuestions",
                table: "QuizAttempts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswers",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "TotalQuestions",
                table: "QuizAttempts");
        }
    }
}
