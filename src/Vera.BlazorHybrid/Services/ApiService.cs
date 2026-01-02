using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Vera.BlazorHybrid.Services;

public class ApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthenticationService _authService;

    public ApiService(IHttpClientFactory httpClientFactory, AuthenticationService authService)
    {
        _httpClientFactory = httpClientFactory;
        _authService = authService;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var client = await GetAuthenticatedClientAsync();
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(responseJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var client = _httpClientFactory.CreateClient("VeraAPI");
        var token = await _authService.GetTokenSilentlyAsync();
        
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        return client;
    }
}
