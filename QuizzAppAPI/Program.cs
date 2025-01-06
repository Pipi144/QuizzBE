using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizzAppAPI.Interfaces;
using QuizzAppAPI.Middleware;
using QuizzAppAPI.Models;
using QuizzAppAPI.QuizAppDbContext;
using QuizzAppAPI.Repositories;
using QuizzAppAPI.Service;

var builder = WebApplication.CreateBuilder(args);


// Add AutoMapper

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true; // Optional for readability
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Define the security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Please enter a valid token"
    });

    // Make sure Swagger knows that this is a required security requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


//Register Db
// Configure Database Connection
builder.Services.AddDbContext<QuizDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Database connection string is not configured.");
    }
    options.UseNpgsql(connectionString);
});

// Register AutoMapper with assembly scanning
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddAuthorization();

// Auth0 configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = $"https://{Environment.GetEnvironmentVariable("AUTH0_DOMAIN")}/";
    options.Audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE");
    
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});
// Scope configuration
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RetrieveUser", policy => 
        policy.RequireClaim("permissions", "read:users"));
    options.AddPolicy("UpdateUser", policy => 
        policy.RequireClaim("permissions", "update:users"));
    options.AddPolicy("DeleteUser", policy => 
        policy.RequireClaim("permissions", "delete:users"));
    options.AddPolicy("GetUserInfo", policy => 
        policy.RequireClaim("permissions", "read:current_user"));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();


// Dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IQuizService, QuizService>();    
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuizAttemptService, QuizAttemptService>();    
builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();


// Configure Auth0Settings
builder.Services.Configure<Auth0Settings>(options =>
{
    options.Domain = Environment.GetEnvironmentVariable("AUTH0_DOMAIN");
    options.Audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE");
    options.ClientId = Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID");
    options.ClientSecret = Environment.GetEnvironmentVariable("AUTH0_CLIENT_SECRET");
    options.Connection = Environment.GetEnvironmentVariable("AUTH0_CONNECTION");
});


builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1"); });
}

app.UseMiddleware<AccessTokenValidationMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>(); // Register global error handler
app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run(); // This line keeps the app running