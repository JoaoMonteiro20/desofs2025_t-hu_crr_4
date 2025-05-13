using EcoImpact.DataModel;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o DbContext com a connection string do appsettings.json
builder.Services.AddDbContext<EcoDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("EcoImpact.API") 
    ));

// Adiciona os serviços de controladores
builder.Services.AddControllers();

// Adiciona suporte a Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ativa Swagger no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware para HTTPS
app.UseHttpsRedirection();

// Middleware de roteamento de controladores
app.UseAuthorization();
app.MapControllers();

app.Run();
