using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rehapp.Infrastructure.Core.Abstractions;

namespace Rehapp.Mobile.ViewModels;


[QueryProperty(nameof(Email), "Email")]
public partial class FinalPasswordRecoveryPageViewModel : BaseViewModel, ITransient
{
    #region fields

    [ObservableProperty]
    private string email;

    #endregion

    [RelayCommand]
    private async Task GoToLoginPageAsync()
    {
        await NavigationService.GoToLoginPageAsync(true);
    }
}
