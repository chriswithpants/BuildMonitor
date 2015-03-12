using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using BuildMonitor.ConfigurationSettings;
using BuildMonitor.Infrastructure.Environments;
using BuildMonitor.Infrastructure.ErrorHandling;
using BuildMonitor.Infrastructure.SignalR;
using ConfigInjector.QuickAndDirty;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Extensions;
using Nancy;
using Nancy.Owin;
using Owin;
using Serilog;
using Serilog.Extras.Web.Enrichers;

namespace BuildMonitor
{
    public class Startup
    {
        private IContainer _container;

        public void Configuration(IAppBuilder app)
        {
            ConfigureLogging();

            Log.Debug("Starting Sample...");

            HookUpToUnhandledExceptionsInTheAppDomain();
            HookUpToUnobservedTaskExceptions();

            _container = IoC.LetThereBeIoC();

            app.UseSerilogErrorHandling();

            app.UseCors(CorsOptions.AllowAll);

            app.UseSignalR(_container);

            app.UseNancy(options =>
            {
                var bootstrapper = new CustomBootstrapper()
                    .UseContainer(_container)
                    .ResolveClaimsPrincipal(ResolveClaimsPrincipal);

                options.Bootstrapper = bootstrapper;
            });

            app.UseStageMarker(PipelineStage.MapHandler);

            app.HookDisposal(TearDown);

            Log.Debug("Sample started...");
        }

        private void TearDown()
        {
            Log.Debug("Stoping Sample...");

            if (_container == null) return;

            try
            {
                Log.Information("Trying to dispose the Autofac Container...");
                _container.Dispose();
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to dispose the Autofac Container: {Message}", e.Message);
            }

            Log.Debug("Sample stopped...");

            _container = null;
        }

        private static void HookUpToUnhandledExceptionsInTheAppDomain()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                try
                {
                    Log.Error((Exception)args.ExceptionObject,
                              "Unhandled Exception: {Message}",
                              ((Exception)args.ExceptionObject).Message);
                }
                // ReSharper disable once UnusedVariable
                catch (Exception e)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                }
            };
        }

        private static void HookUpToUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                try
                {
                    Log.Error(args.Exception,
                              "Unhandled Exception: {Message}",
                              (args.Exception).Message);
                }
                // ReSharper disable once UnusedVariable
                catch (Exception e)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                }
            };
        }

        private ClaimsPrincipal ResolveClaimsPrincipal(NancyContext context)
        {
            var owinEnvironment = context.GetOwinEnvironment();
            var user = owinEnvironment["server.User"] as ClaimsPrincipal;
            return user;
        }

        private void ConfigureLogging()
        {
            var minimumLogLevel = DefaultSettingsReader.Get<MinimumLogLevel>();
            var serverUrl = DefaultSettingsReader.Get<SeqServerUri>().ToString();

            var loggerConfig = new LoggerConfiguration()
                   .MinimumLevel.Is(minimumLogLevel)
                   .Enrich.FromLogContext()
                   .Enrich.WithMachineName()
                   .Enrich.WithThreadId()
                   .Enrich.WithProperty("ApplicationName", "BuildMonitor")
                   .Enrich.WithProperty("ApplicationVersion", GetType().Assembly.GetName().Version)
                   .Enrich.WithProperty("EnvironmentType", AppEnvironment.EnvironmentType)
                   .Enrich.WithProperty("EnvironmentName", AppEnvironment.EnvironmentName)
                   .Enrich.WithProperty("ServiceAccount", Environment.UserName)
                   .Enrich.WithProperty("OSVersion", Environment.OSVersion)
                   .Enrich.With<HttpRequestClientHostIPEnricher>()
                   .Enrich.With<HttpRequestIdEnricher>()
                   .Enrich.With<HttpRequestNumberEnricher>()
                   .Enrich.With<HttpRequestRawUrlEnricher>()
                   .Enrich.With<HttpRequestTraceIdEnricher>()
                   .Enrich.With<HttpRequestTypeEnricher>()
                   .Enrich.With<HttpRequestUrlReferrerEnricher>()
                   .Enrich.With<HttpRequestUserAgentEnricher>()
                   .Enrich.With<UserNameEnricher>()
                   .WriteTo.ColoredConsole()
                   .WriteTo.Seq(serverUrl)
                   .WriteTo.Trace();

            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}