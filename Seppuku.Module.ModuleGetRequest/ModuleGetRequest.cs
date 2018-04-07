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
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();
        public ModuleGetRequest() : base(
            "ModuleGetRequest", 
            "Fires a get request to every endpoint specified", 
            new Dictionary<string, object>
            {
                ["Endpoints"] = new[] { "https://google.com" }
            })
        {
        }

        public override void OnStart(bool triggered)
        {

        }

        public override void OnTrigger()
        {
            var webClient = new WebClient();
            foreach (string uri in ModuleConfig.Get<string[]>("Endpoints"))
            {
                L.Warn("Firing get request to {0}", uri);
                webClient.DownloadString(uri);
            }
        }

        public override void OnStop()
        {

        }
    }
}
