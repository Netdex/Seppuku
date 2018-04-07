using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Nancy;
using Nancy.Hosting.Self;
using Seppuku.Config;

namespace Seppuku.Module.Internal.ModuleWebAPI
{
    [Export(typeof(SeppukuModule))]
    internal class ModuleWebAPI : SeppukuModule
    {
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();
        private NancyHost _host;

        public ModuleWebAPI() : base("ModuleWebAPI", "Enables the web api", new Dictionary<string, object>()
        {
            ["Port"] = 19007L
        })
        {
           
        }

        public override void OnStart(bool triggered)
        {
            var port = (int) ModuleConfig.Get<long>("Port");
            // run the web api
            _host = new NancyHost(new HostConfiguration
            {
                RewriteLocalhost = true,
            }, new Uri($"http://localhost:{port}"));
            _host.Start();
            L.Info($"Running on http://localhost:{port}/");
        }

        public override void OnTrigger()
        {
        }

        public override void OnStop()
        {
            _host.Stop();
        }
    }
}