using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Rehapp.Infrastructure.Core.Extensions;
using System.Reflection;

namespace Rehapp.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("geometria_bold.otf", "geometria_bold");
            fonts.AddFont("geometria_light.otf", "geometria_light");
            fonts.AddFont("geometria_medium.otf", "geometria_medium");
        }).UseMauiCommunityToolkit();

        builder.Services.AddSingleton(new HttpClient());

        var assembly = Assembly.GetExecutingAssembly();

        builder.Services.AddServices(assembly);
        builder.Services.AddViewModels(assembly);
        builder.Services.AddPages(assembly);

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}