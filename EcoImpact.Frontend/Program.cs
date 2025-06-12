using Blazored.LocalStorage;
using EcoImpact.Frontend;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Detecta se está em localhost (dev)
var isDev = builder.HostEnvironment.BaseAddress.Contains("localhost");

// Escolhe ficheiro de configuração
var configFile = isDev ? "appsettings.json" : "appsettings.Production.json";

// Usa HttpClient para ler o ficheiro de configuração
using var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var configJson = await httpClient.GetStringAsync(configFile);

if (string.IsNullOrWhiteSpace(configJson))
    throw new Exception($"{configFile} está vazio ou ausente.");

using var configDoc = JsonDocument.Parse(configJson);
var apiBaseUrl = configDoc.RootElement.GetProperty("ApiBaseUrl").GetString();

if (string.IsNullOrWhiteSpace(apiBaseUrl))
    throw new Exception("ApiBaseUrl não definido.");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl!) });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();