using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NLog.Web;
using NLog;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using NLog.Extensions.Logging;
using CookMaster.Interfaces;
using CookMaster.Services.Factories;
using Microsoft.Extensions.Configuration;


public class Program
{
    private static void Main(string[] args)
    {
        string configFilename = "nlog.config";

#if DEBUG
        configFilename = "nlog.Development.config";
#endif


        var logger = LogManager.Setup()
            .LoadConfigurationFromFile(configFilename)
            .GetCurrentClassLogger();


        logger.Info("CookMaster.WebApp started.");

        var builder = WebApplication.CreateBuilder(args);

        // NLog: Setup NLog for Dependency injection
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        // Nlog DI
        builder.Logging.AddNLog();
        // Respect AppSetting.json Logging section
        builder.Host.UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = false });

        builder.Services.AddHealthChecks();
        // Add services to the container.
        builder.Services.AddControllersWithViews().AddJsonOptions(options =>
        {
            // PascalCase serialization intead of CamelCase
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            // Ignore null values when serialization
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            // Allow trailing commas (helpful for postman dev)
            options.JsonSerializerOptions.AllowTrailingCommas = true;
            // Allow comments (helpful for postman dev)
            options.JsonSerializerOptions.ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip;
        });

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        // In production, the Angular files will be served from this directory
        builder.Services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "ClientApp/dist";
        });

        #region DIs


        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<IStorageFactory>(context =>
        {
            var connStr = builder.Configuration.GetConnectionString("CookMaster");
            return new StorageFactory(connStr, context.GetService<ILoggerFactory>());
        });

        #endregion

        var app = builder.Build();

        var spaSourcePath = "ClientApp";

        if (!app.Environment.IsDevelopment())
        {

            // This handles the static SPA files (ie. in dist folder for angular)
            app.UseSpaStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 30; // 30 days
                    context.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
                }
            });
        }
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();


        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });


        // This handles the fallback to "index.html"
        app.UseSpa(spa =>
        {
            // To learn more about options for serving an Angular SPA from ASP.NET Core,
            // see https://go.microsoft.com/fwlink/?linkid=864501

            spa.Options.SourcePath = spaSourcePath;

            if (app.Environment.IsDevelopment())
            {
                spa.UseAngularCliServer(npmScript: "start");
            }

            spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    // never cache index.html
                    // this if might not be needed... as this is in "DefaultPageStaticFileOptions" 
                    if (context.File.Name == "index.html")
                    {
                        context.Context.Response.Headers.Add(HeaderNames.CacheControl, "no-cache, no-store");
                        context.Context.Response.Headers.Add(HeaderNames.Expires, " -1");
                    }
                }
            };
        });

        app.Run();

        logger.Info("CookMaster.WebApp stoped.");
    }
}
