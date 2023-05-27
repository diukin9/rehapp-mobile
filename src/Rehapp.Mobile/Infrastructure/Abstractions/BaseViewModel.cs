using CommunityToolkit.Mvvm.ComponentModel;

namespace Rehapp.Mobile.Infrastructure.Abstractions;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    protected bool errorIsDisplayed;

    [ObservableProperty]
    protected string errorMessage;

    [ObservableProperty]
    protected bool loaderIsDisplayed;

    protected void ShowLoader()
    {
        //LoaderIsDisplayed = true;
    }

    protected void HideLoader()
    {
        //LoaderIsDisplayed = false;
    }

    protected void ShowError(string message)
    {
        //ErrorMessage = message;
        //ErrorIsDisplayed = true;
    }

    protected bool ShowErrorByCondition(bool condition, string message)
    {
        if (condition) ShowError(message);
        return condition;
    }

    protected void HideError()
    {
        ErrorIsDisplayed = false;
    }

    public virtual async void InitAsync()
    {
        await Task.Delay(1);
    }
}
