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
}