using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using EcoImpact.Frontend.Models;


public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        if (result?.Token is null)
            return false;

        await _localStorage.SetItemAsync("authToken", result.Token);

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result.Token);

        return true;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<bool> IsLoggedInAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users", request);

        if (response.IsSuccessStatusCode)
        {
            return new AuthResult { Success = true };
        }

        var error = await response.Content.ReadAsStringAsync();
        return new AuthResult { Success = false, Error = error };
    }
}
