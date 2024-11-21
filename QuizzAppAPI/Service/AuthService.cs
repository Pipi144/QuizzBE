using System.Text;
using Newtonsoft.Json;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;

namespace QuizzAppAPI.Service;

public class AuthService: IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
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
            var connection = _configuration["Auth0:Connection"];
            var requestBody = new
            {
                username = email,
                password = password,
                client_id = clientId,
                client_secret = clientSecret,
                audience = audience,
                grant_type = "password",
                connection = connection,
            };
            Console.WriteLine(requestBody);
            
            var response = await client.PostAsync(
                $"https://{domain}/oauth/token",
                
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
            );
           
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            Console.WriteLine(response);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

           
            return JsonConvert.DeserializeObject<TokenResponse>(content);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}