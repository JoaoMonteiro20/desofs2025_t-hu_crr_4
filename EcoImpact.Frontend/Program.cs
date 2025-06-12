using Blazored.LocalStorage;
using EcoImpact.Frontend;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Configura diretamente a URL da API aqui:
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:7020") // <-- troca aqui se precisares
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
