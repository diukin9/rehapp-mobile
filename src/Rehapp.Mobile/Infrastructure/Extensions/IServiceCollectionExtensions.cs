using Rehapp.Mobile.Infrastructure.Abstractions;
using System.Reflection;

namespace Rehapp.Mobile.Infrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    #region static fiels

    private static string baseViewModelName => typeof(BaseViewModel).Name;

    private static string contentPageName => typeof(ContentPage).Name;

    private readonly static Type[] dependencyLifecycleInterfaces = new[] 
    { 
        typeof(ITransient), typeof(IScoped), typeof(ISingleton) 
    };

    #endregion

    public static IServiceCollection AddViewModels(this IServiceCollection serviceCollection, Assembly assembly)
    {
        var viewModels = assembly.GetTypes()
            .Where(type => type?.BaseType?.Name == baseViewModelName)
            .ToList();

        foreach (var viewModel in viewModels)
        {
            AddToDependencyContainer(serviceCollection, viewModel);
        }

        return serviceCollection;
    }

    public static IServiceCollection AddPages(this IServiceCollection serviceCollection, Assembly assembly)
    {
        var pages = assembly.GetTypes()
            .Where(type => type?.BaseType?.Name == contentPageName)
            .ToList();

        foreach (var page in pages)
        {
            AddToDependencyContainer(serviceCollection, page);
        }

        return serviceCollection;
    }

    public static IServiceCollection AddServices(this IServiceCollection serviceCollection, Assembly assembly)
    {
        var services = assembly.GetTypes()
            .Where(type => type?.GetInterface(nameof(IService)) is not null)
            .ToList();

        foreach (var service in services)
        {
            AddToDependencyContainer(serviceCollection, service);
        }

        return serviceCollection;
    }

    private static Type GetDependencyLifecycleInterface(Type type)
    {
        var contracts = type.GetInterfaces().ToList();
        return contracts.SingleOrDefault(x => dependencyLifecycleInterfaces.Any(y => y == x));
    }

    private static IServiceCollection AddToDependencyContainer(IServiceCollection services, Type type)
    {
        var contract = GetDependencyLifecycleInterface(type);
        if (contract == typeof(ITransient)) return services.AddTransient(type);
        else if (contract == typeof(IScoped)) return services.AddScoped(type);
        else if (contract == typeof(ISingleton)) return services.AddSingleton(type);
        else throw new NotSupportedException();
    }
}
