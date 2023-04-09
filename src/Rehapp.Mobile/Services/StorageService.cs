using Rehapp.Mobile.Infrastructure.Abstractions;
using Rehapp.Mobile.Models;
using Rehapp.Mobile.Platforms;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

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

        var accessToken = await GetAccessTokenFromSecureStorageAsync();
        if (accessToken is not null) return internalResponse.Success(accessToken);

        ClearAccessTokenDataFromSecureStorage();

        var refreshToken = await GetRefreshTokenFromSecureStorageAsync();
        if (refreshToken is not null)
        {
            var updateExpiredTokenInternalResponse = await UpdateExpiredTokenAsync(accessToken, refreshToken);
            if (updateExpiredTokenInternalResponse.IsSuccess)
            {
                await SetAccessTokenInSecureStorageAsync(updateExpiredTokenInternalResponse.Data);
            }

            if (updateExpiredTokenInternalResponse.Data?.AccessToken is not null)
            {
                return internalResponse.Success(accessToken);
            }
            else return internalResponse.Failure(updateExpiredTokenInternalResponse.Exception);
        }

        ClearRefreshTokenDataFromSecureStorage();

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

        await UpdateTokenInSecureStorageAsync(getTokenInternalResponse.Data);

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

        await UpdateTokenInSecureStorageAsync(getTokenInternalResponse.Data);

        return internalResponse.Success();
    }

    #region privates

    private async Task UpdateTokenInSecureStorageAsync(Token token)
    {
        await SetRefreshTokenInSecureStorageAsync(token);
        await SetAccessTokenInSecureStorageAsync(token);
    }

    private async Task<string> GetAccessTokenFromSecureStorageAsync()
    {
        return await SecureStorage.Default.GetAsync(STORAGE_ACCESS_TOKEN_KEY);
    }

    private async Task<string> GetRefreshTokenFromSecureStorageAsync()
    {
        return await SecureStorage.Default.GetAsync(STORAGE_REFRESH_TOKEN_KEY);
    }

    public void ClearAccessTokenDataFromSecureStorage()
    {
        SecureStorage.Default.Remove(STORAGE_ACCESS_TOKEN_KEY);
    }

    private void ClearRefreshTokenDataFromSecureStorage()
    {
        SecureStorage.Default.Remove(STORAGE_REFRESH_TOKEN_KEY);
    }

    private async Task SetAccessTokenInSecureStorageAsync(Token token)
    {
        await SecureStorage.Default.SetAsync(STORAGE_ACCESS_TOKEN_KEY, token.AccessToken);
    }

    private async Task SetRefreshTokenInSecureStorageAsync(Token token)
    {
        await SecureStorage.Default.SetAsync(STORAGE_REFRESH_TOKEN_KEY, token.RefreshToken);
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
                ? internalResponse.Failure()
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
            //callback будет иметь значение "rehapp://"
            var callback = $"{WebAuthenticationCallbackActivity.CALLBACK_SCHEME}://";
            //authUrl будет иметь значение "https://localhost:7031/api/v1/security/token/Yandex?callback=rehapp://"
            var authUrl = new Uri(API_LOGIN_BY_PROVIDER(scheme, callback));
            //через волшебный WebAuthenticator пробую аутентифицироваться
            var response = await WebAuthenticator.AuthenticateAsync(authUrl, new Uri(callback));
            //из ответа потом достаю токены
            response.Properties.TryGetValue("accessToken", out var accessToken);
            response.Properties.TryGetValue("refreshToken", out var refreshToken);
            //если токенов нет - сообщаю об ошибке
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                return internalResponse.Failure();
            }
            //some logics here...
            return internalResponse.Success(new Token
            {
                AccessToken = WebUtility.UrlDecode(accessToken),
                RefreshToken = WebUtility.UrlDecode(refreshToken)
            });
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"{scheme.ToUpper()} | Unable to get token: {exception.Message}");
            return internalResponse.Failure(exception);
        }
    }

    #endregion
}
