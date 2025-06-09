using Mars.Host.Data.Contexts.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyMarsPlugin.Data.Entities;

namespace MyMarsPlugin.Data;

public partial class MyMarsPluginDbContext : PluginDbContextBase
{
    public override string PluginName => MainMyMarsPlugin.PluginPackageName;

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

    public new static MyMarsPluginDbContext CreateInstance(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<MyMarsPluginDbContext>();

        builder.UseNpgsql(connectionString);
        builder.UseSnakeCaseNamingConvention();
        builder.EnableDetailedErrors();

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
