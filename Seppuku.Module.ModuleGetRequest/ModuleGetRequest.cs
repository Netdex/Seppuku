using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Seppuku.Module.ModuleGetRequest
{
    public class ModuleGetRequest : SeppukuModule
    {
        private static Dictionary<string, object> DefaultConf = new Dictionary<string, object>()
        {
            ["Endpoints"] = new[] {"https://google.com"}
        };

        public ModuleGetRequest() : base("ModuleGetRequest", "Fires a get request to every endpoint specified",
            DefaultConf)
        {
        }

        public override void OnStart()
        {
            
        }

        public override void OnTrigger()
        {
            var webClient = new WebClient();
            foreach (string uri in (string[]) this.Configuration.Conf["Endpoints"])
            {
                webClient.DownloadString(uri);
            }
        }

        public override void OnStop()
        {
            
        }
    }
}
