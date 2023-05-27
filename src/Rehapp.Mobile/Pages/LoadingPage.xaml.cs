using Rehapp.Mobile.Infrastructure.Abstractions;
using Rehapp.Mobile.ViewModels;
//using RehApp.Infrastructure.Common.Interfaces;

namespace Rehapp.Mobile.Pages;

public partial class LoadingPage : ContentPage, ISingleton
{
	public LoadingPage(LoadingPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}