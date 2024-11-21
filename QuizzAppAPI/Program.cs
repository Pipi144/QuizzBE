using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Mappers;
using QuizzAppAPI.QuizAppDbContext;
using QuizzAppAPI.Service;

var builder = WebApplication.CreateBuilder(args);


// Add AutoMapper
builder.Services.AddAutoMapper(typeof(UserMapper));
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Register Db
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization();

// Auth0 configuration
var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://dev-hpou0jp8hflr02wz.au.auth0.com/";
    options.Audience = "https://localhost:7285/";
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});
// Scope configuration
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ReadUsers", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:users", domain)));
    options.AddPolicy("EditUsers", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:users", domain)));
    options.AddPolicy("DeleteUsers", policy => policy.Requirements.Add(new 
        HasScopeRequirement("delete:users", domain)));
    options.AddPolicy("ReadQuiz", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:quiz", domain)));
    options.AddPolicy("EditQuiz", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:quiz", domain)));
    options.AddPolicy("DeleteQuiz", policy => policy.Requirements.Add(new 
        HasScopeRequirement("delete:quiz", domain)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(options =>
// {
//     options.Authority = "https://dev-hpou0jp8hflr02wz.au.auth0.com/";
//     options.Audience = "https://localhost:7285/";
// });

// Dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run(); // This line keeps the app running