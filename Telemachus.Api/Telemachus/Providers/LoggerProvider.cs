using System;
using System.IO;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Formatting.Compact;

using Telemachus.Models;

public static class LoggerProvider
{
    public static ILogger CreateAppLogger(IConfiguration configuration, string environment)
    {
        var logConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext();
        var vessel = configuration.GetSection("VesselDetails").Get<VesselDetails>();

        if (environment == "Development" || vessel?.Prefix.ToLower() == "dev")
        {
            var dirInfo = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Telemachus"));
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            logConfig = logConfig.MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
                .Enrich.WithProperty("Application", "Telemachus")
                .WriteTo.Console(outputTemplate: "{Timestamp:dd/MM/yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .WriteTo.Debug(outputTemplate: "{Timestamp:dd/MM/yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
                .WriteTo.File(path: Path.Combine(dirInfo.FullName, "Telemachus-.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, fileSizeLimitBytes: 10485760, rollOnFileSizeLimit: true, outputTemplate: "{Timestamp:dd/MM/yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .WriteTo.File(new CompactJsonFormatter(), Path.Combine(dirInfo.FullName, "Telemachus-.json"), rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 10485760, rollOnFileSizeLimit: true, retainedFileCountLimit: 7)
                .WriteTo.Seq("http://localhost:5341", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning);
        }
        else
        {
            var dirInfo = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Telemachus"));
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            logConfig = logConfig.MinimumLevel.Error()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Error)
                .Enrich.WithProperty("Application", "Telemachus")
                .WriteTo.File(
                    path: Path.Combine(dirInfo.FullName, "Telemachus-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 10485760,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:dd/MM/yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}");

            if (vessel != null)
            {
                logConfig.WriteTo.Console(outputTemplate: "{Timestamp:dd/MM/yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}");
            }
        }

        return logConfig.CreateLogger();
    }

    public static ILogger CreateAccessLogger(IConfiguration configuration, string environment)
    {
        var logConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext();

        var dirInfo = new DirectoryInfo(environment == "Development"
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Telemachus")
            : Path.Combine(Path.GetTempPath(), "Telemachus"));

        if (!dirInfo.Exists)
        {
            dirInfo.Create();
        }

        if (environment == "Development")
        {
            logConfig = logConfig.MinimumLevel.Information();
        }
        else
        {
            logConfig = logConfig.MinimumLevel.Information();
        }

        logConfig = logConfig
            .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(i => i.Level < Serilog.Events.LogEventLevel.Warning)
            .WriteTo.File(
                path: Path.Combine(dirInfo.FullName, "Telemachus-Access-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: environment == "Development" ? 1 : 7,
                fileSizeLimitBytes: 10485760,
                rollOnFileSizeLimit: true,
                outputTemplate: "{Timestamp:dd/MM/yyyy HH:mm:ss} [{Level}] {Message} [ClientIP: {ClientIP}] [UserAgent: {UserAgent}] {NewLine}"
            ))
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(i => i.Level >= Serilog.Events.LogEventLevel.Warning)
                .WriteTo.File(
                    path: Path.Combine(dirInfo.FullName, "Telemachus-Access-Error-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: environment == "Development" ? 1 : 7,
                    fileSizeLimitBytes: 10485760,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:dd/MM/yyyy HH:mm:ss} [{Level}] {Message} [ClientIP: {ClientIP}] [UserAgent: {UserAgent}] {NewLine}{Exception} {NewLine}"
                ));



        return logConfig.CreateLogger();
    }
}
