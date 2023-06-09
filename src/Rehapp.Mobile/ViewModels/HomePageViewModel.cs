﻿using CommunityToolkit.Mvvm.Input;
using Rehapp.Infrastructure.Core.Abstractions;

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
