using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rehapp.Mobile.Infrastructure.Abstractions;
using Rehapp.Mobile.Infrastructure.Extensions;

namespace Rehapp.Mobile.ViewModels;

public partial class PasswordRecoveryPageViewModel : BaseViewModel, ITransient
{
    #region services

    private readonly AccountService accountService;

    #endregion

    #region fields

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private bool errorIsDisplayed;

    [ObservableProperty]
    private string errorMessage;

    [ObservableProperty]
    private bool loaderIsDisplayed;

    #endregion

    public PasswordRecoveryPageViewModel(AccountService accountService)
    {
        this.accountService = accountService;
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await NavigationService.GoBackAsync(true);
    }

    [RelayCommand]
    private async Task ResetPasswordAsync()
    {
        ErrorIsDisplayed = false;
        LoaderIsDisplayed = true;

        await Task.Delay(100);

        if (string.IsNullOrEmpty(Email))
        {
            LoaderIsDisplayed = false;
            ErrorMessage = "Необходимо заполнить все поля";
            ErrorIsDisplayed = true;
            return;
        }

        if (!Email.IsEmail())
        {
            LoaderIsDisplayed = false;
            ErrorMessage = "Неверный формат почтового ящика";
            ErrorIsDisplayed = true;
            return;
        }

        var sendMailResponse = await accountService.SendMailToRecoveryPasswordAsync(Email);

        LoaderIsDisplayed = false;

        if (!sendMailResponse.IsSuccess)
        {
            ErrorMessage = "Письмо не отправлено. Попробуйте позже";
            ErrorIsDisplayed = true;
            return;
        }

        await NavigationService.GoToFinalPasswordRecoveryPageAsync(true, new Dictionary<string, object>()
        {
            { "Email", Email }
        });
    }
}
