using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rehapp.Infrastructure.Abstractions;
using Rehapp.Mobile.Popups;
//using RehApp.Infrastructure.Common;
//using RehApp.Infrastructure.Common.Interfaces;

namespace Rehapp.Mobile.ViewModels;

public partial class LoginPageViewModel : BaseViewModel, ITransient
{
    #region services

    private readonly StorageService storageService;

    #endregion

    #region properties

    public bool LogInCommandIsNotRunning => !LogInCommand.IsRunning;
    public bool LogInByProviderCommandIsNotRunning => !LogInByProviderCommand.IsRunning;

    #endregion

    #region fields

    [ObservableProperty]
    private string login;

    [ObservableProperty]
    private string password;

    #endregion

    public LoginPageViewModel(StorageService storageService)
    {
        this.storageService = storageService;
    }

    [RelayCommand]
    private async Task GoToPasswordRecoveryPageAsync()
    {
        await NavigationService.GoToPasswordRecoveryPageAsync(true);
    }

    [RelayCommand]
    private async Task GoToRegistrationPageAsync()
    {
        await OpenChoosingAccountTypePopupAsync();
    }

    [RelayCommand]
    private async Task LogInAsync()
    {
        HideError();
        ShowLoader();

        await Task.Delay(100);

        Login = Login?.Trim();

        #region form validation

        var formValidationConditions = new[]
        {
            KeyValuePair.Create(
                key: string.IsNullOrEmpty(Login),
                value: $"Необходимо заполнить поле 'Логин'"),
            KeyValuePair.Create(
                key : string.IsNullOrEmpty(Password),
                value: $"Необходимо заполнить поле 'Пароль'")
        };

        foreach (var kvp in formValidationConditions)
        {
            if (ShowErrorByCondition(kvp.Key, kvp.Value))
            {
                HideLoader();
                return;
            }
        }

        #endregion

        var internalResponse = await storageService.UpdateTokenAsync(Login, Password);

        HideLoader();

        if (!internalResponse.IsSuccess && internalResponse.Exception?.GetType() == typeof(TaskCanceledException))
        {
            ShowError("Ошибка! Повторите позднее..");
            return;
        }
        else if (!internalResponse.IsSuccess)
        {
            ShowError("Неверный логин или пароль");
            return;
        }

        await NavigationService.GoToMainPageAsync(true);
    }

    [RelayCommand]
    private async Task LogInByProviderAsync(string provider)
    {
        HideError();
        ShowLoader();

        await Task.Delay(100);

        var internalResponse = await storageService.UpdateTokenAsync(provider);

        HideLoader();

        if (!internalResponse.IsSuccess && internalResponse.Exception.Message == USER_REGISTRATION_REQUIRED)
        {
            await OpenChoosingAccountTypePopupAsync(provider);
        }
        else if (!internalResponse.IsSuccess)
        {
            ShowError("Ошибка! Повторите позднее..");
        }
    }

    private async Task OpenChoosingAccountTypePopupAsync(string provider = null)
    {
        var popup = new ChoosingAccountTypePopup();
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);

        if (popup.IsSuccessful)
        {
            if (!string.IsNullOrEmpty(provider))
            {
                var userResponse = await storageService.GetUserDataFromProviderAsync(provider);

                await NavigationService.GoToRegistrationPageAsync(true, new Dictionary<string, object>
                {
                    { EMAIL, userResponse.Data?.Email },
                    { USERNAME, userResponse.Data?.Username },
                    { FIRSTNAME, userResponse.Data?.FirstName },
                    { SURNAME, userResponse.Data?.Surname },
                    { ACCOUNT_TYPE, popup.AccountType }
                });
            }
            else await NavigationService.GoToRegistrationPageAsync(true, new Dictionary<string, object>
            {
                { ACCOUNT_TYPE, popup.AccountType }
            });
        }
    }
}
