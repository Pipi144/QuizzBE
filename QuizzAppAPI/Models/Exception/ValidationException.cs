namespace QuizzAppAPI.Models;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}