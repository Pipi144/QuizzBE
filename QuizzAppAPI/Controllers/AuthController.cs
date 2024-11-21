using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizzAppAPI.DTO;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Models;
using QuizzAppAPI.QuizAppDbContext;
using QuizzAppAPI.Service;

namespace QuizzAppAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
       private readonly IAuthService _authService;

       public AuthController(IAuthService authService)
       {
           _authService = authService;
       }
       
        
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(LoginDTO data)
        {
            try
            {
                var res = await _authService.Login(data.Email, data.Password);
                if (res == null)
                    return BadRequest(new { message = "Username or password is incorrect" });
                else 
                    return Ok(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "Internal server error");
            }
        }
      
    }
}
