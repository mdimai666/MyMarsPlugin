using Mars.Plugin.Front;
using Mars.Plugin.Front.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyMarsPlugin.Shared.Resources;

namespace MyMarsPlugin.Front;

public class MainMyMarsPluginFront : IWebAssemblyPluginFront
{
    public void ConfigureServices(WebAssemblyHostBuilder builder)
    {
#if DEBUG
        Console.WriteLine("> MyMarsPlugin plugin ConfigureServices!");
#endif
    }

    public void ConfigureApplication(WebAssemblyHost app)
    {
#if DEBUG
        Console.WriteLine("> MyMarsPlugin plugin ConfigureApplication!" + Locale.Username);
#endif
        app.Services.AutoFrontRegisterHelper([GetType().Assembly]);
    }

}
