namespace QuizzAppAPI.Models;

public class UnauthorizedAccessException: Exception
{
    public UnauthorizedAccessException(string message) : base(message) { }
}