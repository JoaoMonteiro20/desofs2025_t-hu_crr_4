using Blazored.LocalStorage;
using EcoImpact.Frontend;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Detecta se est� em localhost (dev)
var isDev = builder.HostEnvironment.BaseAddress.Contains("localhost");

// Escolhe ficheiro de configura��o
var configFile = isDev ? "appsettings.json" : "appsettings.Production.json";

// Usa HttpClient para ler o ficheiro de configura��o
using var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var configJson = await httpClient.GetStringAsync(configFile);

if (string.IsNullOrWhiteSpace(configJson))
    throw new Exception($"{configFile} est� vazio ou ausente.");

using var configDoc = JsonDocument.Parse(configJson);
var apiBaseUrl = configDoc.RootElement.GetProperty("ApiBaseUrl").GetString();

if (string.IsNullOrWhiteSpace(apiBaseUrl))
    throw new Exception("ApiBaseUrl n�o definido.");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl!) });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();