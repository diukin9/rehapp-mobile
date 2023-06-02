using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rehapp.Infrastructure.Abstractions;
using Rehapp.Infrastructure.Extensions;
//using RehApp.Infrastructure.Common;
//using RehApp.Infrastructure.Common.Extensions;
//using RehApp.Infrastructure.Common.Interfaces;

namespace Rehapp.Mobile.ViewModels;

public partial class PasswordRecoveryPageViewModel : BaseViewModel, ITransient
{
    #region services

    private readonly AccountService accountService;

    #endregion

    #region fields

    [ObservableProperty]
    private string email;

    #endregion

    #region properties

    public bool ResetPasswordCommandIsNotRunning => !ResetPasswordCommand.IsRunning;

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
        HideError();
        ShowLoader();

        await Task.Delay(100);

        #region form validation

        var formValidationConditions = new[]
        {
            KeyValuePair.Create(
                key: string.IsNullOrEmpty(Email),
                value: "Необходимо заполнить поле 'Имя'"),
            KeyValuePair.Create(
                key: !Email.IsEmail(),
                value: "Неверный формат почтового ящика"),
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

        var sendMailResponse = await accountService.SendMailToRecoveryPasswordAsync(Email);

        HideLoader();

        //TODO Такого пользователя нет

        if (ShowErrorByCondition(
            condition: !sendMailResponse.IsSuccess,
            message: "Письмо не отправлено. Попробуйте позже"))
        {
            return;
        }

        await NavigationService.GoToFinalPasswordRecoveryPageAsync(true, new Dictionary<string, object>()
        {
            { "Email", Email }
        });
    }
}
