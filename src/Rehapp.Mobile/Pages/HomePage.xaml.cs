using Rehapp.Mobile.Infrastructure.Abstractions;
using Rehapp.Mobile.ViewModels;
//using RehApp.Infrastructure.Common.Interfaces;

namespace Rehapp.Mobile.Pages;

public partial class HomePage : ContentPage, ISingleton
{
	public HomePage(HomePageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}