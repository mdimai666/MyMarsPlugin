using Mars.Host.Shared.Services;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Implements;
using Mars.Plugin.Abstractions;
using Mars.Plugin.PluginHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyMarsPlugin;
using MyMarsPlugin.Data;
using MyMarsPlugin.Front;
using MyMarsPlugin.Front.Nodes;
using MyMarsPlugin.Front.Nodes.Forms;
using MyMarsPlugin.FrontImplement;
using MyMarsPlugin.Services;
using MyMarsPlugin.Shared.Options;
using MyMarsPlugin.Shared.Resources;

[assembly: WebApplicationPlugin(typeof(MainMyMarsPlugin))]

namespace MyMarsPlugin;

public class MainMyMarsPlugin : WebApplicationPlugin
{
    public const string PluginPackageName = "MyPluginCompany.MyMarsPlugin";

    public override void ConfigureWebApplicationBuilder(WebApplicationBuilder builder, PluginSettings settings)
    {
        builder.Services.AddSingleton<MyPluginService>();

        //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        //builder.Services.AddTransient(sp => MyMarsPluginDbContext.CreateInstance(connectionString));

        //Migration(app, settings).Wait();
    }

    public override void ConfigureWebApplication(WebApplication app, PluginSettings settings)
    {
        NodesLocator.RegisterAssembly(typeof(MyPluginNode).Assembly);
        NodeFormsLocator.RegisterAssembly(typeof(MyPluginNodeForm).Assembly);
        NodeImplementFabirc.RegisterAssembly(typeof(MyPluginNodeImpl).Assembly);

        var logger = MarsLogger.GetStaticLogger<MainMyMarsPlugin>();
        logger.LogWarning($"> {PluginPackageName} - Work!" + Locale.Username);

#if DEBUG
        app.UseDevelopingServePluginFilesDefinition(this.GetType().Assembly, settings, [typeof(MainMyMarsPluginFront).Assembly]);
#endif

        //some option
        var optionService = app.Services.GetRequiredService<IOptionService>();
        optionService.RegisterOption<MyPluginOption>(appendToInitialSiteData: true);
        //optionService.SetConstOption(new Example1PluginConstOptionForFront() { ForFrontValue = "123" }, appendToInitialSiteData: true);
    }

    public async Task Migration(WebApplication app, PluginSettings settings)
    {
        var conn = app.Configuration.GetConnectionString("DefaultConnection")!;
        using var ef = MyMarsPluginDbContext.CreateInstance(conn);

        var pendingMigrations = (await ef.Database.GetPendingMigrationsAsync()).ToList();
        var migrations = ef.Database.GetMigrations();
        var appliedMigrations = await ef.Database.GetAppliedMigrationsAsync();

        if (!migrations.Any())
            throw new Exception("Migrations not exist or DbContext configure invalid!");

        if (pendingMigrations.Count() > 0)
        {
            Console.WriteLine($"Start PLUGIN {PluginPackageName} migrate...");
            ef.Database.Migrate();
            Console.WriteLine($"Migrate PLUGIN {PluginPackageName} complete.");
        }

        //SomeSeed.SeedFirstData(ef, serviceScope.ServiceProvider, app.Configuration, settings.ContentRootPath).Wait();
    }

}
