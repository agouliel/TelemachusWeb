using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Debugging;

using Telemachus.Business.Interfaces.Reports;
using Telemachus.Models;

namespace Telemachus
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            if (environment == "Development")
            {
                SelfLog.Enable(msg => Console.WriteLine(msg));

            }

            CultureInfo culture;
            culture = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            //environment = "";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = LoggerProvider.CreateAppLogger(configuration, environment);

            try
            {
                string asciiArt = @"
_______    _                           _
|__   __|  | |                         | |
    | | ___| | ___ _ __ ___   __ _  ___| |__  _   _ ___
    | |/ _ \ |/ _ \ '_ ` _ \ / _` |/ __| '_ \| | | / __|
    | |  __/ |  __/ | | | | | (_| | (__| | | | |_| \__ \
    |_|\___|_|\___|_| |_| |_|\__,_|\___|_| |_|\__,_|___/
";
                Console.WriteLine(asciiArt);
                try
                {
                    var vesselDetails = configuration.GetSection("VesselDetails").Get<VesselDetails>();

                    var host = CreateHostBuilder(args).Build();

                    Console.WriteLine("Starting up server...");

                    if (vesselDetails != null && vesselDetails.ListenPort != 0)
                    {
                        var address = $"http://localhost:{vesselDetails.ListenPort}";
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Opening browser on: \"{address}\"");
                        Console.ResetColor();
                        OpenBrowser(address);
                    }

                    if (environment == "Development")
                    {

                        using (var scope = host.Services.CreateScope())
                        {
                            var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();
                            //await reportService.PostProcess();
                            // return;
                        }
                    }

                    await host.RunAsync();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("An unhandled exception occurred:");
                    Console.ResetColor();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("\nPress Enter to exit...");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        private static void OpenBrowser(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", url);
                else
                    Process.Start("xdg-open", url);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to open browser.");
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        var isProduction = context.HostingEnvironment.IsProduction();
                        var vesselDetails = context.Configuration.GetSection("VesselDetails").Get<VesselDetails>();
                        if (vesselDetails != null)
                        {
                            options.ListenAnyIP(vesselDetails.ListenPort);
                        }
                    });
                }).ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables();
                });
    }
}
