using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizzAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class Removeuserstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_CreatedByUserId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_Users_AttemptById",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Users_CreatedByUserId",
                table: "Quizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.AddColumn<int>(
                name: "UserRole",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_User_CreatedByUserId",
                table: "Questions",
                column: "CreatedByUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_User_AttemptById",
                table: "QuizAttempts",
                column: "AttemptById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_User_CreatedByUserId",
                table: "Quizzes",
                column: "CreatedByUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_User_CreatedByUserId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_User_AttemptById",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_User_CreatedByUserId",
                table: "Quizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_CreatedByUserId",
                table: "Questions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Users_AttemptById",
                table: "QuizAttempts",
                column: "AttemptById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Users_CreatedByUserId",
                table: "Quizzes",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
