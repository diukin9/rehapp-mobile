using Rehapp.Mobile.Infrastructure.Abstractions;
using Rehapp.Mobile.ViewModels;

namespace Rehapp.Mobile.Pages;

public partial class PasswordRecoveryPage : ContentPage, ITransient
{
	public PasswordRecoveryPage(PasswordRecoveryPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    private void HideKeyboard(object sender, TappedEventArgs e)
    {
        EmailEntry.IsEnabled = false;
        EmailEntry.IsEnabled = true;
    }

    private void HideKeyboard(object sender, EventArgs e)
    {
        EmailEntry.IsEnabled = false;
        EmailEntry.IsEnabled = true;
    }
}