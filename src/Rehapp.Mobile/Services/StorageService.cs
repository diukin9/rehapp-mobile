using Rehapp.Infrastructure.Core.Abstractions;
using Rehapp.Infrastructure.Core.Models;
using Rehapp.Mobile.Models;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

#if ANDROID
using Rehapp.Mobile.Platforms;
#endif

namespace Rehapp.Mobile.Services;

public class StorageService : IService, ITransient
{
    private readonly HttpClient httpClient;

    public StorageService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<InternalResponse<string>> GetTokenAsync()
    {
        var internalResponse = new InternalResponse<string>();

        var accessToken = await GetAsync(ACCESS_TOKEN);
        if (accessToken is not null) return internalResponse.Success(accessToken);

        Remove(ACCESS_TOKEN);

        var refreshToken = await GetAsync(REFRESH_TOKEN);
        if (refreshToken is not null)
        {
            var updateExpiredTokenInternalResponse = await UpdateExpiredTokenAsync(accessToken, refreshToken);
            if (updateExpiredTokenInternalResponse?.Data?.AccessToken is not null)
            {
                await SetAsync(ACCESS_TOKEN, updateExpiredTokenInternalResponse.Data.AccessToken);
                return internalResponse.Success(accessToken);
            }
            else return internalResponse.Failure(updateExpiredTokenInternalResponse.Exception);
        }

        Remove(REFRESH_TOKEN);

        return internalResponse.Failure();
    }

    public async Task<InternalResponse> UpdateTokenAsync(string login, string password)
    {
        var internalResponse = new InternalResponse();

        var getTokenInternalResponse = await GetTokenAsync(login, password);
        if (!getTokenInternalResponse.IsSuccess)
        {
            return internalResponse.Failure(getTokenInternalResponse.Exception);
        }

        await SetAsync(ACCESS_TOKEN, getTokenInternalResponse.Data.AccessToken);
        await SetAsync(REFRESH_TOKEN, getTokenInternalResponse.Data.RefreshToken);

        return internalResponse.Success();
    }

    public async Task<InternalResponse> UpdateTokenAsync(string scheme)
    {
        var internalResponse = new InternalResponse();

        var getTokenInternalResponse = await GetTokenAsync(scheme);
        if (!getTokenInternalResponse.IsSuccess)
        {
            return internalResponse.Failure(getTokenInternalResponse.Exception);
        }

        await SetAsync(ACCESS_TOKEN, getTokenInternalResponse.Data.AccessToken);
        await SetAsync(REFRESH_TOKEN, getTokenInternalResponse.Data.RefreshToken);

        return internalResponse.Success();
    }

    public async Task<InternalResponse<ExternalUser>> GetUserDataFromProviderAsync(string provider)
    {
        var response = new InternalResponse<ExternalUser>();

        var user = new ExternalUser
        {
            Email = await SecureStorage.Default.GetAsync(EmailFromProvider(provider)),
            Username = await SecureStorage.Default.GetAsync(UsernameFromProvider(provider)),
            FirstName = await SecureStorage.Default.GetAsync(FirstnameFromProvider(provider)),
            Surname = await SecureStorage.Default.GetAsync(SurnameFromProvider(provider))
        };

        if (string.Join(user.Email, user.Surname, user.FirstName, user.Username) == string.Empty)
        {
            response.Failure();
        }

        return response.Success(user);
    }

    public async Task SetUserDataFromProdiverAsync(string provider, ExternalUser user)
    {
        var pairs = new Dictionary<string, string>()
        {
            { EmailFromProvider(provider), user.Email },
            { UsernameFromProvider(provider), user.Username },
            { FirstnameFromProvider(provider), user.FirstName },
            { SurnameFromProvider(provider), user.Surname }
        };

        foreach (var pair in pairs)
        {
            if (!string.IsNullOrEmpty(pair.Value)) await SetAsync(pair.Key, pair.Value);
            else Remove(pair.Key);
        }
    }

    public void ClearAccessTokenDataFromSecureStorage()
    {
        SecureStorage.Default.Remove(ACCESS_TOKEN);
    }

    #region privates

    private static async Task<string> GetAsync(string key)
    {
        return await SecureStorage.Default.GetAsync(key);
    }

    private static async Task SetAsync(string key, string value)
    {
        await SecureStorage.Default.SetAsync(key, value);
    }

    private static void Remove(string key)
    {
        SecureStorage.Default.Remove(key);
    }

    private async Task<InternalResponse<Token>> UpdateExpiredTokenAsync(string expiredAccessToken, string refreshToken)
    {
        var internalResponse = new InternalResponse<Token>();

        var body = JsonSerializer.Serialize(new
        {
            expired_access_token = expiredAccessToken,
            refresh_token = refreshToken
        });
        var requestContent = new StringContent(body, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PutAsync(API_LOGIN, requestContent);
            var content = await response.Content.ReadAsStreamAsync();
            var token = await JsonSerializer.DeserializeAsync<Token>(content);
            return token?.RefreshToken is null || token?.AccessToken is null 
                ? internalResponse.Failure() 
                : internalResponse.Success(token);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to update token: {ex.Message}");
            return internalResponse.Failure(ex);
        }
    }

    private async Task<InternalResponse<Token>> GetTokenAsync(string login, string password)
    {
        var internalResponse = new InternalResponse<Token>();

        var body = JsonSerializer.Serialize(new { login, password });
        var requestContent = new StringContent(body, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync(API_LOGIN, requestContent);
            var content = await response.Content.ReadAsStreamAsync();
            var token = await JsonSerializer.DeserializeAsync<Token>(content);
            return token?.RefreshToken is null || token?.AccessToken is null
                ? internalResponse.Failure(new InvalidDataException())
                : internalResponse.Success(token);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get token: {ex.Message}");
            return internalResponse.Failure(ex);
        }
    }

    private async Task<InternalResponse<Token>> GetTokenAsync(string scheme)
    {
        var internalResponse = new InternalResponse<Token>();
        
        try
        {
#if ANDROID
            var callback = $"{WebAuthenticationCallbackActivity.CALLBACK_SCHEME}://";
            var authUrl = new Uri(API_LOGIN_BY_PROVIDER(scheme, callback));
            var response = await WebAuthenticator.AuthenticateAsync(authUrl, new Uri(callback));
            var props = response.Properties.Select(x => new { x.Key, Value = WebUtility.UrlDecode(x.Value) }).ToList();

            if (props.Any(x => x.Key == HRESULT)) return internalResponse.Failure();
            else if (props.FirstOrDefault(x => x.Key == MESSAGE).Value.ToLower() == USER_REGISTRATION_REQUIRED)
            {
                await SetUserDataFromProdiverAsync(scheme, new ExternalUser
                {
                    FirstName = props.FirstOrDefault(x => x.Key == FIRSTNAME)?.Value,
                    Email = props.FirstOrDefault(x => x.Key == EMAIL)?.Value,
                    Username = props.FirstOrDefault(x => x.Key == USERNAME)?.Value,
                    Surname = props.FirstOrDefault(x => x.Key == SURNAME)?.Value
                });
                
                return internalResponse.Failure(new InvalidOperationException(USER_REGISTRATION_REQUIRED));
            }
            else
            {
                var token = new Token
                {
                    AccessToken = props.FirstOrDefault(x => x.Key == ACCESS_TOKEN)?.Value,
                    RefreshToken = props.FirstOrDefault(x => x.Key == REFRESH_TOKEN)?.Value,
                };
                
                if (string.IsNullOrEmpty(token.AccessToken) || string.IsNullOrEmpty(token.RefreshToken))
                {
                    return internalResponse.Failure();
                }

                return internalResponse.Success(token);
            }
#else 
            return await Task.FromResult(internalResponse.Failure(new NotImplementedException()));
#endif
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"{scheme.ToUpper()} | Unable to get token: {exception.Message}");
            return internalResponse.Failure(exception);
        }
    }

    #endregion
}
