using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Seppuku.Switch;

namespace Seppuku.Endpoint
{
    public class IndexEndpoint : NancyModule
    {
        public IndexEndpoint()
        {
            Get["/"] = _ => $"{Assembly.GetExecutingAssembly().GetName()}";
            Get["/remain"] = _ =>
            {
                var timeleft = SwitchControl.TimeLeft();
                if (SwitchControl.Expired())
                    return "Expired";

                return $"Remaining time until expiry: {timeleft}";
            };
            Get["/reset"] = _ =>
            {
                if (SwitchControl.Expired())
                    return "Cannot reset expired timer";

                SwitchControl.Reset();
                return "Reset successful";
            };
        }
    }
}
