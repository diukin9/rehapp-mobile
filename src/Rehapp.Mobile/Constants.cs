namespace Rehapp.Mobile;

public static class Constants
{
    #region api

#if ANDROID && DEBUG
    public const string API_URL = "http://10.0.2.2:7030/api/v1";
#elif DEBUG
    public const string API_URL = "https://localhost:7031/api/v1";
#else
    public const string API_URL = "<production address>/api/v1";
#endif

    public const string API_LOGIN = $"{API_URL}/security/token";

    public static string API_LOGIN_BY_PROVIDER(string scheme, string callback) => $"{API_URL}/security/token/{scheme}?callback={callback}"; 

#endregion

    #region storage keys

    public const string STORAGE_REFRESH_TOKEN_KEY = "refresh_token";
    public const string STORAGE_ACCESS_TOKEN_KEY = "access_token";

    #endregion
}
