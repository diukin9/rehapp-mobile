using Rehapp.Infrastructure.Abstractions;
using Rehapp.Mobile.Models;
//using RehApp.Infrastructure.Common.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Rehapp.Mobile.Services;

public class ApiService : IService, ITransient
{
    private readonly StorageService storageService;
    private readonly HttpClient httpClient;

    public ApiService(StorageService storageService, HttpClient httpClient)
    {
        this.storageService = storageService;
        this.httpClient = httpClient;
    }

    public async Task<InternalResponse<T>> GetAsync<T>(string url, bool throwAuthToken = false) where T : class
    {
        var internalResponse = new InternalResponse<T>();
        try
        {
            var response = await SendRequestAsync(throwAuthToken, x => x.GetAsync(url));
            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<T>(content);
            return internalResponse.Success(result);
        }
        catch(Exception exception) { return internalResponse.Failure(exception); }
    }

    public async Task<InternalResponse<string>> GetAsStringAsync(string url, bool throwAuthToken = false)
    {
        var internalResponse = new InternalResponse<string>();
        try
        {
            var response = await SendRequestAsync(throwAuthToken, x => x.GetAsync(url));
            var result = await response.Content.ReadAsStringAsync();
            return internalResponse.Success(result);
        }
        catch (Exception exception) { return internalResponse.Failure(exception); }
    }

    public async Task<InternalResponse<HttpResponseMessage>> PostAsync<TBody>(string url, TBody body, bool throwAuthToken = false)
    {
        var internalResponse = new InternalResponse<HttpResponseMessage>();
        try
        {
            var content = BuildStringContent(body);
            var response = await SendRequestAsync(throwAuthToken, x => x.PostAsync(url, content));
            return internalResponse.Success(response);
        }
        catch (Exception exception) { return internalResponse.Failure(exception); }
    }

    public async Task<InternalResponse<HttpResponseMessage>> PutAsync<T>(string url, T body, bool throwAuthToken = false)
    {
        var internalResponse = new InternalResponse<HttpResponseMessage>();
        try
        {
            var content = BuildStringContent(body);
            var response = await SendRequestAsync(throwAuthToken, x => x.PutAsync(url, content));
            return internalResponse.Success(response);
        }
        catch (Exception exception) { return internalResponse.Failure(exception); }
    }

    public async Task<InternalResponse<HttpResponseMessage>> DeleteAsync(string url, bool throwAuthToken = false)
    {
        var internalResponse = new InternalResponse<HttpResponseMessage>();
        try
        {
            var response = await SendRequestAsync(throwAuthToken, x => x.DeleteAsync(url));
            return internalResponse.Success(response);
        }
        catch (Exception exception) { return internalResponse.Failure(exception); }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        bool throwAuthToken, 
        Func<HttpClient, Task<HttpResponseMessage>> action)
    {
        if (throwAuthToken) await ThrowAuthorizationTokenAsync();
        var response = await action.Invoke(httpClient);

        if (throwAuthToken && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            storageService.ClearAccessTokenDataFromSecureStorage();
            await ThrowAuthorizationTokenAsync();
            response = await action.Invoke(httpClient);
        }

        RemoveAuthorizationTokenFromHttpClient();
        return response;
    }

    private async Task ThrowAuthorizationTokenAsync()
    {
        var getTokenInternalResponse = await storageService.GetTokenAsync();
        if (!getTokenInternalResponse.IsSuccess) await NavigationService.GoToLoginPageAsync();

        var authHeader = new AuthenticationHeaderValue("Bearer", getTokenInternalResponse.Data);
        httpClient.DefaultRequestHeaders.Authorization = authHeader;
    }

    private StringContent BuildStringContent<T>(T item)
    {
        const string mediaType = "application/json";
        var content = JsonSerializer.Serialize(item);
        var requestContent = new StringContent(content, Encoding.UTF8, mediaType);
        return requestContent;
    }

    private void RemoveAuthorizationTokenFromHttpClient()
    {
        httpClient.DefaultRequestHeaders.Authorization = default;
    }
}
