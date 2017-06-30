using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Seppuku.Module.ModuleGetRequest
{
    [Export(typeof(SeppukuModule))]
    public class ModuleGetRequest : SeppukuModule
    {

        public ModuleGetRequest() : base(
            "ModuleGetRequest", 
            "Fires a get request to every endpoint specified", 
            new Dictionary<string, object>
            {
                ["Endpoints"] = new[] { "https://google.com" }
            })
        {
        }

        public override void OnStart()
        {

        }

        public override void OnTrigger()
        {
            var webClient = new WebClient();
            foreach (string uri in Configuration.Get<string[]>("Endpoints", DefaultConf))
            {
                Log("Firing get request to {0}", uri);
                webClient.DownloadString(uri);
            }
        }

        public override void OnStop()
        {

        }
    }
}
