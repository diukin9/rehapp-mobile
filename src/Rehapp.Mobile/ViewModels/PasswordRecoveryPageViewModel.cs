using CommunityToolkit.Mvvm.Input;
using Rehapp.Mobile.Infrastructure.Abstractions;

namespace Rehapp.Mobile.ViewModels;

public partial class PasswordRecoveryPageViewModel : BaseViewModel, ITransient
{
    [RelayCommand]
    private async Task GoBackAsync()
    {
        await NavigationService.GoBackAsync(true);
    }
}
