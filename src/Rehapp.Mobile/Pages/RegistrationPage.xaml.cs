using Rehapp.Mobile.Infrastructure.Abstractions;
using Rehapp.Mobile.ViewModels;

namespace Rehapp.Mobile.Pages;

public partial class RegistrationPage : ContentPage, ITransient
{
	public RegistrationPage(RegistrationPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}