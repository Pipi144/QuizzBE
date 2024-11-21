using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Models;

namespace QuizzAppAPI.Interfaces;

public interface IAuthRepository
{
    Task<TokenResponse?> Login(string email, string password);
}