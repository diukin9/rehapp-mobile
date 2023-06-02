using Rehapp.Infrastructure.Abstractions;
using Rehapp.Mobile.ViewModels;
//using RehApp.Infrastructure.Common.Interfaces;

namespace Rehapp.Mobile.Pages;

public partial class FinalPasswordRecoveryPage : ContentPage, ITransient
{
	public FinalPasswordRecoveryPage(FinalPasswordRecoveryPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}