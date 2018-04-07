using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Seppuku.Module.Internal.Proxy;

namespace Seppuku.Module.Proxy
{
    [Export(typeof(SeppukuModule))]
    public class ModuleProxy : SeppukuModule
    {
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        private TcpForwarderSlim _tcp;
        private Thread _listenThread;

        public ModuleProxy() : base(
            "ModuleProxy",
            "Proxies all requests, changing endpoint when triggered",
            new Dictionary<string, object>
            {
                ["ListenAddress"] = "0.0.0.0",
                ["ListenPort"] = 19080L,
                ["EndpointAddressNormal"] = "151.80.40.155",
                ["EndpointPortNormal"] = 80L,
                ["EndpointAddressTriggered"] = "151.80.40.155",
                ["EndpointPortTriggered"] = 81L
            })
        {
            
        }

        public override void OnStart(bool triggered)
        {
            L.Trace("Listening at {0}:{1}", 
                ModuleConfig.Get<string>("ListenAddress"), ModuleConfig.Get<long>("ListenPort"));
            if (triggered)
            {
                OnTrigger();
            }
            else
            {
                OnReset();
            }

        }

        private void ProxyEndpoint(string laddr, int lport, string eaddr, int eport)
        {
            if (_listenThread == null)
            {
                _tcp = new TcpForwarderSlim();
                _listenThread = new Thread(() =>
                {
                    try
                    {
                        _tcp.Start(new IPEndPoint(IPAddress.Parse(laddr), lport));
                    }
                    catch (SocketException ex)
                    {
                        L.Error(ex, "TCP Forwarder encountered exception");
                    }

                    L.Trace("Forwarder terminated from thread");
                    _listenThread = null;
                });
                _listenThread.Start();
            }
            L.Trace("Setting TCP forwarder endpoint to {0}:{1}", eaddr, eport);
            _tcp.Remote = new IPEndPoint(IPAddress.Parse(eaddr), eport);
            
        }
        public override void OnTrigger()
        {
            ProxyEndpoint(
                ModuleConfig.Get<string>("ListenAddress"), (int)ModuleConfig.Get<long>("ListenPort"), 
                ModuleConfig.Get<string>("EndpointAddressTriggered"), (int)ModuleConfig.Get<long>("EndpointPortTriggered"));
        }

        public override void OnReset()
        {
            ProxyEndpoint(
                ModuleConfig.Get<string>("ListenAddress"), (int)ModuleConfig.Get<long>("ListenPort"),
                ModuleConfig.Get<string>("EndpointAddressNormal"), (int)ModuleConfig.Get<long>("EndpointPortNormal"));
        }

        public override void OnStop()
        {
            if (_tcp != null)
            {
                L.Trace("Terminating forwarder from module stop");
                _tcp.Stop();
            }
        }
    }
}
