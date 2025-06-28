using System.Text.Json;

namespace BlazorApp1.Services;

public class EasyAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;
    private readonly ILogger<EasyAuthService> _logger;

    public EasyAuthService(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<EasyAuthService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
        _logger = logger;
    }

    public EasyAuthInfo GetEasyAuthInfo()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            return new EasyAuthInfo { IsAuthenticated = false };
        }

        var headers = context.Request.Headers;
        
        return new EasyAuthInfo
        {
            IsAuthenticated = headers.ContainsKey("X-MS-CLIENT-PRINCIPAL-ID"),
            PrincipalId = headers["X-MS-CLIENT-PRINCIPAL-ID"].FirstOrDefault(),
            PrincipalName = headers["X-MS-CLIENT-PRINCIPAL-NAME"].FirstOrDefault(),
            IdentityProvider = headers["X-MS-CLIENT-PRINCIPAL-IDP"].FirstOrDefault(),
            AadIdToken = headers["X-MS-TOKEN-AAD-ID-TOKEN"].FirstOrDefault(),
            AadAccessToken = headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"].FirstOrDefault(),
            AadRefreshToken = headers["X-MS-TOKEN-AAD-REFRESH-TOKEN"].FirstOrDefault(),
            ClientPrincipal = headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault()
        };
    }

    public async Task<DetailedAuthInfo?> GetDetailedAuthInfoAsync()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                return null;
            }

            // Build the URL for the auth endpoint
            var scheme = context.Request.Scheme;
            var host = context.Request.Host;
            var authMeUrl = $"{scheme}://{host}/.auth/me";

            // Make request to /.auth/me endpoint
            var response = await _httpClient.GetAsync(authMeUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var authData = JsonSerializer.Deserialize<List<AuthMeResponse>>(json, options);
                return authData?.FirstOrDefault()?.ToDetailedAuthInfo();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving detailed auth info");
        }

        return null;
    }

    /// <summary>
    /// Gets the access token for use in API calls.
    /// This token should be used in the Authorization header as: Bearer {token}
    /// </summary>
    public async Task<string?> GetAccessTokenAsync()
    {
        // First try to get from headers (if token store is enabled in App Service)
        var authInfo = GetEasyAuthInfo();
        if (!string.IsNullOrEmpty(authInfo.AadAccessToken))
        {
            return authInfo.AadAccessToken;
        }

        // If not in headers, get from /.auth/me endpoint
        var detailedInfo = await GetDetailedAuthInfoAsync();
        return detailedInfo?.AccessToken;
    }

    /// <summary>
    /// Gets the ID token which contains user claims and identity information.
    /// </summary>
    public async Task<string?> GetIdTokenAsync()
    {
        // First try to get from headers (if token store is enabled in App Service)
        var authInfo = GetEasyAuthInfo();
        if (!string.IsNullOrEmpty(authInfo.AadIdToken))
        {
            return authInfo.AadIdToken;
        }

        // If not in headers, get from /.auth/me endpoint
        var detailedInfo = await GetDetailedAuthInfoAsync();
        return detailedInfo?.IdToken;
    }
}

public class EasyAuthInfo
{
    public bool IsAuthenticated { get; set; }
    public string? PrincipalId { get; set; }
    public string? PrincipalName { get; set; }
    public string? IdentityProvider { get; set; }
    public string? AadIdToken { get; set; }
    public string? AadAccessToken { get; set; }
    public string? AadRefreshToken { get; set; }
    public string? ClientPrincipal { get; set; }
}

public class DetailedAuthInfo
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? IdentityProvider { get; set; }
    public List<ClaimInfo>? UserClaims { get; set; }
    public string? AccessToken { get; set; }
    public string? IdToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresOn { get; set; }
}

public class ClaimInfo
{
    public string? Type { get; set; }
    public string? Value { get; set; }
}

internal class AuthMeResponse
{
    public string? access_token { get; set; }
    public string? id_token { get; set; }
    public string? refresh_token { get; set; }
    public string? expires_on { get; set; }
    public string? provider_name { get; set; }
    public string? user_id { get; set; }
    public List<UserClaim>? user_claims { get; set; }

    public DetailedAuthInfo ToDetailedAuthInfo()
    {
        return new DetailedAuthInfo
        {
            UserId = user_id,
            UserName = user_claims?.FirstOrDefault(c => c.typ == "name")?.val,
            IdentityProvider = provider_name,
            UserClaims = user_claims?.Select(c => new ClaimInfo { Type = c.typ, Value = c.val }).ToList(),
            AccessToken = access_token,
            IdToken = id_token,
            RefreshToken = refresh_token,
            ExpiresOn = DateTime.TryParse(expires_on, out var exp) ? exp : null
        };
    }
}

internal class UserClaim
{
    public string? typ { get; set; }
    public string? val { get; set; }
}