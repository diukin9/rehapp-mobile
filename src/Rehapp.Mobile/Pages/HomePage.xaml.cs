using Rehapp.Mobile.Infrastructure.Abstractions;
using Rehapp.Mobile.ViewModels;

namespace Rehapp.Mobile.Pages;

public partial class HomePage : ContentPage, ISingleton
{
	public HomePage(HomePageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}