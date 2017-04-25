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
    [Export(typeof(TriggerModule))]
    class ModuleWebAPI : TriggerModule
    {
        private static readonly Dictionary<string, object> DefaultConf = new Dictionary<string, object>();

        private NancyHost _host;
        public ModuleWebAPI() : base("ModuleWebAPI", "Enables the web api", DefaultConf)
        {

        }

        public override void OnStart()
        {
            // run the web api
            _host = new NancyHost(new DefaultNancyBootstrapper(), new HostConfiguration
            {
                RewriteLocalhost = false
            }, new Uri($"http://localhost:{(int)Conf.I.Conf["Port"]}"));
            _host.Start();
            Log($"Running on http://localhost:{(int)Conf.I.Conf["Port"]}/");
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
