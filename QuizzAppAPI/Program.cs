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

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true; // Optional for readability
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
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register AutoMapper with assembly scanning
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



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


// Dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection("Auth0"));
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
    });
}
app.UseMiddleware<AccessTokenValidationMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();  // Register global error handler
app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run(); // This line keeps the app running