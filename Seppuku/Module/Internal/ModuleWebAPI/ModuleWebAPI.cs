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
        private NancyHost _host;

        public ModuleWebAPI() : base("ModuleWebAPI", "Enables the web api", new Dictionary<string, object>())
        {
            
        }

        public override void OnStart()
        {
            var port = (int) Seppuku.Config.Configuration.Get<long>("Port", 19007);
            // run the web api
            _host = new NancyHost(new HostConfiguration
            {
                RewriteLocalhost = false,
            }, new Uri($"http://localhost:{port}"));
            _host.Start();
            Log($"Running on http://localhost:{port}/");
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