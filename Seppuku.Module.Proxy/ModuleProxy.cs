using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Seppuku.Module.Proxy
{
    [Export(typeof(SeppukuModule))]
    public class ModuleProxy : SeppukuModule
    {
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        private TcpForwarderSlim _tcp;

        public ModuleProxy() : base(
            "ModuleProxy",
            "Proxies all requests, changing endpoint when triggered",
            new Dictionary<string, object>
            {
                ["Endpoints"] = new[] { "https://google.com" }
            })
        {
            _tcp = new TcpForwarderSlim();
        }

        public override void OnStart()
        {
            _tcp.Start(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345), new IPEndPoint(IPAddress.Parse("107.6.106.82"), 80));
        }

        public override void OnTrigger()
        {

        }

        public override void OnStop()
        {
            _tcp.Stop();
        }
    }
}
