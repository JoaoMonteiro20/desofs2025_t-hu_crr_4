using EcoImpact.API.Mapper;
using EcoImpact.API.Services;
using EcoImpact.DataModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// JWT config
var jwtSecret = Environment.GetEnvironmentVariable("JWT")
    ?? throw new Exception("JWT não definido nas variáveis de ambiente.");
var key = Encoding.UTF8.GetBytes(jwtSecret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role,
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse(); // impede resposta automática padrão
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var response = new
            {
                error = "Não autenticado",
                detail = "Token inválido ou ausente."
            };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var response = new
            {
                error = "Acesso negado",
                detail = "Você não tem permissão para executar esta ação."
            };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    };
});

builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHabitTypeService, HabitTypeService>();
builder.Services.AddScoped<IUserChoiceService, UserChoiceService>();
builder.Services.AddScoped<IHabitTypeMapper, HabitTypeMapper>();
builder.Services.AddScoped<IUserValidator, UserValidator>();
builder.Services.AddScoped<IUserMapper, UserMapper>();
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://desofs2025-t-hu-crr-4-1.onrender.com",
            "http://localhost:7001",
            "https://localhost:7001"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
// Database
builder.Services.AddDbContext<EcoDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql =>
        {
            npgsql.MigrationsAssembly("EcoImpact.API");
        }));

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//  Swagger + JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EcoImpact.API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insere 'Bearer' + espaço + token JWT.\nEx: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

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
            new string[] { }
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoImpact.API v1");
});

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EcoDbContext>();
    dbContext.Database.Migrate(); // Aplica as migrations automaticamente
}
app.MapControllers();

app.Run();