using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;
using QuizzAppAPI.QuizAppDbContext;

namespace QuizzAppAPI.Repositories;

public class AuthRepository: IAuthRepository
{
    private readonly IConfiguration _configuration;

    public AuthRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public async Task<TokenResponse?> Login(string email, string password)
    {
        try
        {
            var client = new HttpClient();

            var domain = _configuration["Auth0:Domain"];
            var clientId = _configuration["Auth0:ClientId"];
            var clientSecret = _configuration["Auth0:ClientSecret"];
            var audience = _configuration["Auth0:Audience"];

            var requestBody = new
            {
                grant_type = "password",
                username = email,
                password = password,
                client_id = clientId,
                client_secret = clientSecret,
                audience = audience,
                scope = "openid profile email"
            };

            var response = await client.PostAsync(
                $"https://{domain}/oauth/token",
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenResponse>(content);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}