using Rehapp.Infrastructure.Core.Abstractions;
using Rehapp.Mobile.ViewModels;
namespace Rehapp.Mobile.Pages;

public partial class RegistrationPage : ContentPage, ITransient
{
	public RegistrationPage(RegistrationPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    private void HideKeyboard(object sender, TappedEventArgs e)
    {
        EmailEntry.IsEnabled = false;
        EmailEntry.IsEnabled = true;

        SurnameEntry.IsEnabled = false;
        SurnameEntry.IsEnabled = true;

        FirstNameEntry.IsEnabled = false;
        FirstNameEntry.IsEnabled = true;

        UsernameEntry.IsEnabled = false;
        UsernameEntry.IsEnabled = true;

        PasswordEntry.IsEnabled = false;
        PasswordEntry.IsEnabled = true;

        PasswordConfirmationEntry.IsEnabled = false;
        PasswordConfirmationEntry.IsEnabled = true;
    }
}