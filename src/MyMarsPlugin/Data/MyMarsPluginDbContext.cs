using Mars.Host.Data.Constants;
using Mars.Host.Data.Contexts;
using Mars.Host.Data.Contexts.Abstractions;
using Mars.Host.Data.Options;
using Mars.Host.Data.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using MyMarsPlugin.Data.Entities;

namespace MyMarsPlugin.Data;

public partial class MyMarsPluginDbContext : PluginDbContextBase
{
    public override string SchemaName => MainMyMarsPlugin.PluginPackageName;

    public virtual DbSet<PluginNewsEntity> News { get; set; } = default!;

    public MyMarsPluginDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (optionsBuilder.Options.ContextType != typeof(MyMarsPluginDbContext))
            throw new ArgumentException($"optionsBuilder.Options.ContextType must be '{nameof(MyMarsPluginDbContext)}'. Given '{optionsBuilder.Options.ContextType}'");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

        OnModelCreatingPartial(builder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public static MyMarsPluginDbContext CreateInstance(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<MyMarsPluginDbContext>();

        var connectOpt = new DatabaseConnectionOpt() { ConnectionString = connectionString, ProviderName = DatabaseProviderConstants.PostgreSQL };
        var factory = new MarsDbContextPostgreSQLForPluginFactory(connectOpt, typeof(MyMarsPluginDbContext).Assembly, MainMyMarsPlugin.PluginPackageName);
        factory.OptionsBuilderAction(builder);
        ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(new MarsDbContextOptionExtension(factory));

        return new MyMarsPluginDbContext(builder.Options);
    }
}

public class MyMarsPluginDbContextFactory : IDesignTimeDbContextFactory<MyMarsPluginDbContext>
{
    public MyMarsPluginDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.DesignTime.json")
                        .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        return MyMarsPluginDbContext.CreateInstance(connectionString);
    }
}
