using CommunityToolkit.Mvvm.Input;
using Rehapp.Infrastructure.Abstractions;
//using RehApp.Infrastructure.Common;
//using RehApp.Infrastructure.Common.Interfaces;

namespace Rehapp.Mobile.ViewModels;

public partial class HomePageViewModel : BaseViewModel, ITransient
{
    [RelayCommand]
    private async Task ClearSecureStorageAndGoBackAsync()
    {
        SecureStorage.Default.RemoveAll();
        await NavigationService.GoToLoginPageAsync();
    }
}
