using CommunityToolkit.Mvvm.ComponentModel;

namespace Rehapp.Mobile.Infrastructure.Abstractions;

public abstract partial class BaseViewModel : ObservableObject
{
    public virtual async void InitAsync()
    {
        await Task.Delay(1);
    }
}
