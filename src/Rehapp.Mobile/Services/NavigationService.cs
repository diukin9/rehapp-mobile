using Rehapp.Mobile.Pages;

namespace Rehapp.Mobile.Services;

public static class NavigationService
{
    private static async Task GoToAsync(
        string url,
        bool withAnimation = false,
        Dictionary<string, object> parameters = null)
    {
        if (parameters is null) await Shell.Current.GoToAsync(url, withAnimation);
        else await Shell.Current.GoToAsync(url, withAnimation, parameters);
    }

    public static async Task GoBackAsync(bool withAnimation = false)
    {
        await GoToAsync("..", withAnimation);
    }

    public static async Task GoToLoadingPageAsync(
        bool withAnimation = false,
        Dictionary<string, object> parameters = null)
    {
        await GoToAsync(nameof(LoadingPage), withAnimation, parameters);
    }

    public static async Task GoToMainPageAsync(
        bool withAnimation = false,
        Dictionary<string, object> parameters = null)
    {
        await GoToAsync("//MainPage", withAnimation, parameters);
    }

    public static async Task GoToLoginPageAsync(
        bool withAnimation = false,
        Dictionary<string, object> parameters = null)
    {
        await GoToAsync(nameof(LoginPage), withAnimation, parameters);
    }

    public static async Task GoToPasswordRecoveryPageAsync(
        bool withAnimation = false,
        Dictionary<string, object> parameters = null)
    {
        await GoToAsync(nameof(PasswordRecoveryPage), withAnimation, parameters);
    }

    public static async Task GoToFinalPasswordRecoveryPageAsync(
        bool withAnimation = false,
        Dictionary<string, object> parameters = null)
    {
        await GoToAsync(nameof(FinalPasswordRecoveryPage), withAnimation, parameters);
    }

    public static async Task GoToRegistrationPageAsync(
        bool withAnimation = false,
        Dictionary<string, object> parameters = null)
    {
        await GoToAsync(nameof(RegistrationPage), withAnimation, parameters);
    }
}
