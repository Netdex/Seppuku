using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Logging.Configuration;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seppuku.Switch;

namespace Seppuku.Endpoint
{
    public class IndexEndpoint : NancyModule
    {
        private readonly Dictionary<string, Func<dynamic, JObject>> JSONGet = new Dictionary<string, Func<dynamic, JObject>>();

        public IndexEndpoint()
        {
            JSONGet["/"] = _ =>
            {
                JObject jo = new JObject
                {
                    ["application"] = Assembly.GetExecutingAssembly().GetName().Name,
                    ["version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                };
                return jo;
            };

            JSONGet["/remain"] = _ =>
            {
                var timeleft = SwitchControl.TimeLeft();
                JObject jo = new JObject();
                if (SwitchControl.Expired())
                {
                    jo["verbose"] = "expired";
                    jo["remaining"] = 0;
                }
                else
                {
                    jo["verbose"] = timeleft.ToString() + " remaining";
                    jo["remaining"] = timeleft.TotalSeconds;
                }
                return jo;
            };

            JSONGet["/reset/{secret}"] = param =>
            {
                JObject jo = new JObject();
                if (SwitchControl.Authorized(param.secret))
                {
                    if (SwitchControl.Expired())
                    {
                        jo["verbose"] = "cannot reset expired timer";
                        jo["status"] = -1;
                    }
                    else
                    {
                        SwitchControl.Reset();
                        jo["verbose"] = "reset successful";
                        jo["status"] = 0;
                    }
                }
                else
                {
                    jo["verbose"] = "unauthorized";
                    jo["status"] = -999;
                }
                return jo;
            };

            foreach (var k in JSONGet)
            {
                Get[k.Key] = param =>
                {
                    var jo = (JObject) k.Value(param);
                    return Response.AsText(jo.ToString(Formatting.None));
                };
            }
        }
    }
}
