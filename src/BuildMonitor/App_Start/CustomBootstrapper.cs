using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using BuildMonitor.Infrastructure;
using BuildMonitor.Infrastructure.Authentication;
using BuildMonitor.Infrastructure.Environments;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Nancy.IO;
using Serilog;

namespace BuildMonitor
{
    public class CustomBootstrapper : AutofacNancyBootstrapper
    {
        private IContainer _container;
        private Func<NancyContext, ClaimsPrincipal> _claimsPrincipalResolver;

        public CustomBootstrapper UseContainer(IContainer containerToUse)
        {
            if (ApplicationContainer != null)
                throw new Exception("The ApplicationContainer already exists! This method should be called before the ApplicationContainer is initialized by Nancy.");

            _container = containerToUse;
            return this;
        }

        public CustomBootstrapper ResolveClaimsPrincipal(Func<NancyContext, ClaimsPrincipal> resolver)
        {
            _claimsPrincipalResolver = resolver;
            return this;
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new PathProvider(); }
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return _container ?? base.GetApplicationContainer();
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            conventions.StaticContentsConventions.AddDirectory(@"App", "App");
            conventions.StaticContentsConventions.AddDirectory(@"Scripts", "Scripts");
            conventions.StaticContentsConventions.AddDirectory(@"Content", "Content");
            conventions.StaticContentsConventions.AddDirectory(@"Fonts", "Fonts");
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            pipelines.BeforeRequest += async (nancyContext, token) =>
            {
                await LogRequestStart(container, nancyContext, token);

                // Resolve and assign the CurrentUser
                if (_claimsPrincipalResolver != null)
                {
                    var principal = _claimsPrincipalResolver(nancyContext);
                    if (principal != null && principal.Identity.IsAuthenticated)
                    {
                        var claimsIdentity = (ClaimsIdentity)principal.Identity;
                        nancyContext.CurrentUser = new AuthenticatedUser(claimsIdentity);
                    }
                }

                // Otherwise continue with processing this request
                return null;
            };

            pipelines.AfterRequest +=
                (nancyContext, token) => LogRequestComplete(container, nancyContext, token);

            pipelines.OnError +=
                (nancyContext, exception) => LogRequestError(container, nancyContext, exception);
        }

        private Task<Response> LogRequestStart(ILifetimeScope lifetimeScope, NancyContext context, CancellationToken ct)
        {
            return Task.Run(() =>
                            {
                                var logger = lifetimeScope.Resolve<ILogger>();
                                var requestLogContext = lifetimeScope.Resolve<RequestLogContext>();
                                var requestBody = context.Request.Body.ReadAsString();
                                logger.Debug("Begin {HttpMethod} to {RequestUrl} ({RequestId}) {@RequestBody}",
                                             context.Request.Method,
                                             context.Request.Url,
                                             requestLogContext.RequestId,
                                             requestBody);
                                return context.Response;
                            },
                            ct);
        }

        private Task LogRequestComplete(ILifetimeScope lifetimeScope, NancyContext context, CancellationToken ct)
        {
            return Task.Run(() =>
                            {
                                var logger = lifetimeScope.Resolve<ILogger>();
                                logger.Debug("End {HttpMethod} to {RequestUrl}", context.Request.Method, context.Request.Url);
                            },
                            ct);
        }

        private Response LogRequestError(ILifetimeScope lifetimeScope, NancyContext context, Exception exception)
        {
            var logger = lifetimeScope.Resolve<ILogger>();
            logger.Error(exception, "Error on {HttpMethod} to {RequestUrl}: {Message}", context.Request.Method, context.Request.Url, exception.Message);
            return context.Response;
        }
    }

    public class PathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            if (AppEnvironment.IsLocal())
            {
                var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..");
                return Path.GetFullPath(fullPath);
            }
            else
            {
                var fullPath = AppDomain.CurrentDomain.BaseDirectory;
                return Path.GetFullPath(fullPath);
            }
        }
    }

    /// <see cref="http://www.heikura.me/tip-nancyfx-read-request-body-as-string" />
    public static class RequestBodyExtensions
    {
        public static string ReadAsString(this RequestStream requestStream)
        {
            using (var reader = new StreamReader(requestStream))
            {
                var result = reader.ReadToEnd();
                requestStream.Seek(0, SeekOrigin.Begin);
                return result;
            }
        }
    }
}