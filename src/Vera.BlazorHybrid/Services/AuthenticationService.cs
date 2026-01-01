using Microsoft.Identity.Client;

namespace Vera.BlazorHybrid.Services;

public class AuthenticationService
{
    private readonly IPublicClientApplication _msalClient;
    private string? _accessToken;

    public AuthenticationService()
    {
        _msalClient = PublicClientApplicationBuilder
            .Create("YOUR_CLIENT_ID") // From Azure AD
            .WithAuthority("https://login.microsoftonline.com/YOUR_TENANT_ID")
            .WithRedirectUri("msal://auth")
            .Build();
    }

    public async Task<string?> LoginAsync()
    {
        try
        {
            var scopes = new[] { "api://YOUR_API_CLIENT_ID/access_as_user" };
            var result = await _msalClient
                .AcquireTokenInteractive(scopes)
                .ExecuteAsync();

            _accessToken = result.AccessToken;
            return _accessToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> GetTokenSilentlyAsync()
    {
        if (_accessToken != null)
            return _accessToken;

        try
        {
            var accounts = await _msalClient.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();

            if (firstAccount == null)
                return await LoginAsync();

            var scopes = new[] { "api://YOUR_API_CLIENT_ID/access_as_user" };
            var result = await _msalClient
                .AcquireTokenSilent(scopes, firstAccount)
                .ExecuteAsync();

            _accessToken = result.AccessToken;
            return _accessToken;
        }
        catch
        {
            return await LoginAsync();
        }
    }

    public async Task LogoutAsync()
    {
        var accounts = await _msalClient.GetAccountsAsync();
        foreach (var account in accounts)
        {
            await _msalClient.RemoveAsync(account);
        }
        _accessToken = null;
    }
}
