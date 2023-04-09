namespace Rehapp.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await NavigationService.GoToLoadingPageAsync();
    }
}