using System;
using BuildMonitor.ConfigurationSettings;
using ConfigInjector.QuickAndDirty;
using Microsoft.Owin.Hosting;
using Topshelf;

namespace BuildMonitor
{
    public class BuildMonitorService : ServiceControl
    {
        private IDisposable _service;

        public bool Start(HostControl hostControl)
        {
            var baseUri = DefaultSettingsReader.Get<BaseUri>();

            _service = WebApp.Start<Startup>(baseUri);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            var service = _service;
            if (service != null) _service.Dispose();

            return true;
        }
    }
}