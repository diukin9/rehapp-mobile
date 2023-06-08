using Rehapp.Infrastructure.Core.Abstractions;
using Rehapp.Mobile.ViewModels;
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