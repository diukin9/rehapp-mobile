using Rehapp.Infrastructure.Core.Abstractions;
using Rehapp.Mobile.ViewModels;

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