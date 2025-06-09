using AppFront.Main.OptionEditForms;
using Mars.Nodes.Core;
using Mars.Plugin.Front.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyMarsPlugin.Front.Nodes;
using MyMarsPlugin.Front.Nodes.Forms;
using MyMarsPlugin.Front.OptionForms;
using MyMarsPlugin.Shared.Resources;

namespace MyMarsPlugin.Front;

public class MainMyMarsPluginFront : IWebAssemblyPluginFront
{
    public void ConfigureServices(WebAssemblyHostBuilder builder)
    {
#if DEBUG
        Console.WriteLine("> MyMarsPlugin plugin ConfigureServices!");
#endif

        NodesLocator.RegisterAssembly(typeof(MyPluginNode).Assembly);
        NodeFormsLocator.RegisterAssembly(typeof(MyPluginNodeForm).Assembly);
        OptionsFormsLocator.RegisterAssembly(typeof(MyPluginOptionEditForm).Assembly);
    }

    public void ConfigureApplication(WebAssemblyHost app)
    {
#if DEBUG
        Console.WriteLine("> MyMarsPlugin plugin ConfigureApplication!" + Locale.Username);
#endif
    }

}
