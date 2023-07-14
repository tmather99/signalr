using System.Reflection;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Compact;
using signalr;
using signalr.HealthChecks;
using signalr.Hubs;
using signalr.Models;
using signalr.Repositories;
using signalr.SerilogExtensions;

LoggingLevelSwitch loggingLevelSwitch = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

// Bootstrap logger, will capture any crashes and log it until the initialization is completed.
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateBootstrapLogger();

try
{
    var entryAssembly = Assembly.GetEntryAssembly();
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);
    builder.Services.AddSingleton<IAuctionRepo, AuctionMemoryRepo>();
    builder.Services.AddHostedService<HostedService>();
    builder.Services.AddHealthChecks().AddCheck<SignalrHealthCheck>("SignalR");

    var seqEndpoint = builder.Configuration["SEQ_ENDPOINT"] ?? "http://localhost:5342";

    builder.Host.UseSerilog((context, config) =>
        {
            config.Enrich.With<TraceIdEnricher>();
            config.MinimumLevel.ControlledBy(loggingLevelSwitch);
            config.ReadFrom.Configuration(context.Configuration);
            config.WriteTo.Console(new CompactJsonFormatter());
            config.WriteTo.Seq(seqEndpoint);
        },
        preserveStaticLogger: false,
        writeToProviders: false);

    var app = builder.Build();

    // Get the entry assembly title and version.
    var appName = entryAssembly?.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "missing title";
    var versionNumber = entryAssembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.ToString() ?? "missing version";

    Log.Logger.Information("{appName} version {versionNumber} is starting.", appName, versionNumber);

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    // Set Serilog as the default logging provider for the asp.net stack.
    app.UseSerilogRequestLogging();
    app.UseHealthChecks("/health");
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    
    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapGet("/auctions", (IAuctionRepo auctionRepo) => { return auctionRepo.GetAll(); });
    app.MapPost("auction/{auctionId}/newbid", (int auctionId, int currentBid, IAuctionRepo auctionRepo) => { auctionRepo.NewBid(auctionId, currentBid); });
    app.MapPost("auction", (Auction auction, IAuctionRepo auctionRepo, IHubContext<AuctionHub> hubContext) =>
    {
        auctionRepo.AddAuction(auction);
        hubContext.Clients.All.SendAsync("ReceiveNewAuction", auction);
    });

    app.MapHub<AuctionHub>("/auctionHub");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during startup.");
}
finally
{
    Log.CloseAndFlushAsync();
}