using System.Text;
using FeatureFlagApi.Data;
using FeatureFlagApi.Helpers;
using FeatureFlagApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using TaskFlowLiteApi.Extensions;

// 1. Define configuration constants
var corsConfig = "corsConfig";

// 2. Create the WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);

// 3. Add services to the DI container

// Get JWT settings from configuration
var jwtSettings =
    builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings are not configured properly.");
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter 'Bearer [space] and then your token in the text input below.\n\nExample: 'Bearer abc123'",
        }
    );
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", document), new List<string>() },
    });
});

builder.Services.AddScoped<IFeatureFlagService, FeatureFlagService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddNpgsql<FeatureFlagDbContext>(connectionString); // Use PostgreSQL for production

// 5. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: corsConfig,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowedToAllowWildcardSubdomains();
        }
    );
});

// Add Authentication
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
            ),
        };
    });

builder.Services.AddAuthorization();

// 6. Build the application
var app = builder.Build();

// 7. Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsConfig);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 8. Run database migrations
await app.MigrateDatabaseAsync();

// 9. Start the application
app.Run();
