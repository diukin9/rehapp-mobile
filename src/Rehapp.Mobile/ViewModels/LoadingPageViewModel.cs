using Rehapp.Infrastructure.Core.Abstractions;

namespace Rehapp.Mobile.ViewModels;

public class LoadingPageViewModel : BaseViewModel, ISingleton
{
    #region services

    private readonly StorageService storageService;

    #endregion

    public LoadingPageViewModel(StorageService storageService)
    {
        this.storageService = storageService;
        GoToStartPageAsync();
    }

    private async void GoToStartPageAsync()
    {
        var token = await storageService.GetTokenAsync();

        if (token?.Data is null)
        {
            await NavigationService.GoToLoginPageAsync();
        }
        else
        {
            await NavigationService.GoToMainPageAsync();
        }
    }
}
