using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Hosting.Self;
using Seppuku.Config;
using Seppuku.Module.Utility;

namespace Seppuku.Module.Internal
{
    [Export(typeof(SeppukuModule))]
    class ModuleWebAPI : SeppukuModule
    {
        private static readonly Dictionary<string, object> DefaultConf = new Dictionary<string, object>();

        private NancyHost _host;
        public ModuleWebAPI() : base("ModuleWebAPI", "Enables the web api", DefaultConf)
        {

        }

        public override void OnStart()
        {
            int port = Conf.Get("Port", 19007);
            // run the web api
            _host = new NancyHost(new DefaultNancyBootstrapper(), new HostConfiguration
            {
                RewriteLocalhost = false
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
