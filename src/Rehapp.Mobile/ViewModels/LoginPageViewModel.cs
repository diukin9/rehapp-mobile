using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rehapp.Mobile.Infrastructure.Abstractions;
using Rehapp.Mobile.Models.Enums;
using Rehapp.Mobile.Popups;

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

    [ObservableProperty]
    private bool errorIsDisplayed;

    [ObservableProperty]
    private string errorMessage;

    [ObservableProperty]
    private bool loaderIsDisplayed;

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
        var popup = new ChoosingAccountTypePopup();
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);

        if (popup.IsSuccessful)
        {
            if (popup.AccountType == AccountType.Patient)
            {
                //TODO change to patient registration page
                await NavigationService.GoToRegistrationPageAsync();
            }
            else
            {
                //TODO change to specialist registration page
                await NavigationService.GoToRegistrationPageAsync();
            }
        }
    }

    [RelayCommand]
    private async Task LogInAsync()
    {
        ErrorIsDisplayed = false;
        LoaderIsDisplayed = true;

        await Task.Delay(100);

        if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password))
        {
            LoaderIsDisplayed = false;
            ErrorMessage = "Необходимо заполнить все поля";
            ErrorIsDisplayed = true;
            return;
        }

        var internalResponse = await storageService.UpdateTokenAsync(Login, Password);

        LoaderIsDisplayed = false;

        if (!internalResponse.IsSuccess && internalResponse.Exception.GetType() == typeof(TaskCanceledException))
        {
            ErrorMessage = "Ошибка! Повторите позднее..";
            ErrorIsDisplayed = true;
            return;
        }
        else if (!internalResponse.IsSuccess)
        {
            ErrorMessage = "Неверный логин или пароль";
            ErrorIsDisplayed = true;
            return;
        }

        await NavigationService.GoToMainPageAsync(true);
    }

    [RelayCommand]
    private async Task LogInByProviderAsync(string provider)
    {
        ErrorIsDisplayed = false;
        LoaderIsDisplayed = true;

        await Task.Delay(100);

        //var internalResponse = await storageService.UpdateTokenAsync(provider);

        LoaderIsDisplayed = false;

        //if (!internalResponse.IsSuccess && незарегистрирован)
        //{
        //    var popup = new ChoosingAccountTypePopup();
        //    await Shell.Current.CurrentPage.ShowPopupAsync(popup);

        //    if (popup.IsSuccessful)
        //    {
        //        if (popup.AccountType == AccountType.Patient)
        //        {
        //            //TODO change to patient registration page
        //            await NavigationService.GoToRegistrationPageAsync();
        //        }
        //        else
        //        {
        //            //TODO change to specialist registration page
        //            await NavigationService.GoToRegistrationPageAsync();
        //        }
        //    }
        //}
        //else if (!internalResponse.IsSuccess)
        //{
        //    ErrorMessage = "Ошибка! Повторите позднее..";
        //    ErrorIsDisplayed = true;
        //    return;
        //}

        //await NavigationService.GoToMainPageAsync(true);
    }
}
