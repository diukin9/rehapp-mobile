namespace Rehapp.Mobile;

public static class Constants
{
    #region pages

    public const string PASSWORD_RECOVERY_PAGE = "temp_path"; //TODO

    #endregion

    #region api

    public const string API_URL = "https://back.Rehapp.apps.maksimemelyanov.ru/api/v1";

    public const string API_LOGIN = $"{API_URL}/security/token";

    public const string API_CITIES = $"{API_URL}/cities";

    public static string API_LOGIN_BY_PROVIDER(string scheme, string callback) => $"{API_URL}/security/token/{scheme}?callback={callback}";

    public const string API_SEND_MAIL_TO_RECOVER_PASSWORD = $"{API_URL}/account/reset-password/send-mail";

    public const string API_REGISTER = $"{API_URL}/account/register";

    #endregion

    #region storage keys

    public static string UsernameFromProvider(string provider) => $"{provider}-{USERNAME}";
    public static string SurnameFromProvider(string provider) => $"{provider}-{SURNAME}";
    public static string FirstnameFromProvider(string provider) => $"{provider}-{FIRSTNAME}";
    public static string EmailFromProvider(string provider) => $"{provider}-{EMAIL}";


    #endregion

    #region common

    public const string USER_REGISTRATION_REQUIRED = "user registration required";
    public const string HRESULT = "hresult";
    public const string MESSAGE = "message";
    public const string USERNAME = "username";
    public const string SURNAME = "surname";
    public const string FIRSTNAME = "firstname";
    public const string EMAIL = "email";
    public const string ACCESS_TOKEN = "accessToken";
    public const string REFRESH_TOKEN = "refreshToken";
    public const string ACCOUNT_TYPE = "accountType";

    #endregion
}
