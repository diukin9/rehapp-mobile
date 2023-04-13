using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Rehapp.Mobile.Pages;

namespace Rehapp.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(PasswordRecoveryPage), typeof(PasswordRecoveryPage));
            Routing.RegisterRoute(nameof(FinalPasswordRecoveryPage), typeof(FinalPasswordRecoveryPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        }

        protected override void OnAppearing()
        {
            Behaviors.Add(new StatusBarBehavior
            {
                StatusBarColor = new Color(58, 58, 58),
                StatusBarStyle = StatusBarStyle.LightContent
            });
            base.OnAppearing();
        }
    }
}