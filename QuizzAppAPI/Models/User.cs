namespace QuizzAppAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Role UserRole { get; set; }
}