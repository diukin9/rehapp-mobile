using Rehapp.Infrastructure.Abstractions;
using Rehapp.Mobile.ViewModels;
//using RehApp.Infrastructure.Common.Interfaces;

namespace Rehapp.Mobile.Pages;

public partial class LoginPage : ContentPage, ITransient
{
    private bool logInByYandexButton_IsAnimating;
    private bool logInByVkButton_IsAnimating;

    public LoginPage(LoginPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    private async void LogInByYandexButton_Clicked(object sender, EventArgs e)
    {
        if (!logInByYandexButton_IsAnimating)
        {
            logInByYandexButton_IsAnimating = true;
            await LogInByYandexButton.ScaleTo(1.07);
            await LogInByYandexButton.ScaleTo(1);
            logInByYandexButton_IsAnimating = false;
        }
    }

    private async void LogInByVkButton_Clicked(object sender, EventArgs e)
    {
        if (!logInByVkButton_IsAnimating)
        {
            logInByVkButton_IsAnimating = true;
            await LogInByVkButton.ScaleTo(1.07);
            await LogInByVkButton.ScaleTo(1);
            logInByVkButton_IsAnimating = false;
        }
    }

    private void HideKeyboard(object sender, TappedEventArgs e)
    {
        LoginEntry.IsEnabled = false;
        LoginEntry.IsEnabled = true;

        PasswordEntry.IsEnabled = false;
        PasswordEntry.IsEnabled = true;
    }

    private void HideKeyboard(object sender, EventArgs e)
    {
        PasswordEntry.IsEnabled = false;
        PasswordEntry.IsEnabled = true;
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}