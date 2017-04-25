using System;
using System.Collections.Generic;
using System.Reflection;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seppuku.Switch;

namespace Seppuku.Module.Internal.Endpoint
{
    /// <summary>
    /// Defines all web api endpoints
    /// </summary>
    public class IndexEndpoint : NancyModule
    {

        public IndexEndpoint()
        {
            Get["/"] = _ =>
            {
                JObject jo = new JObject
                {
                    ["application"] = Assembly.GetExecutingAssembly().GetName().Name,
                    ["version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                };
                return Response.AsText(jo.ToString(Formatting.None));
            };

            Get["/remain"] = _ =>
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
                return Response.AsText(jo.ToString(Formatting.None));
            };

            Get["/reset/{secret}"] = param =>
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
                return Response.AsText(jo.ToString(Formatting.None));
            };

            Get["/trigger/{secret}"] = param =>
            {
                JObject jo = new JObject();
                if (SwitchControl.Authorized(param.secret))
                {
                    if (SwitchControl.Expired())
                    {
                        jo["verbose"] = "cannot trigger expired timer";
                        jo["status"] = -1;
                    }
                    else
                    {
                        SwitchControl.Trigger();
                        jo["verbose"] = "trigger successful";
                        jo["status"] = 0;
                    }
                }
                else
                {
                    jo["verbose"] = "unauthorized";
                    jo["status"] = -999;
                }
                return Response.AsText(jo.ToString(Formatting.None));
            };

#if DEBUG
            Get["/debug/reset"] = _ =>
            {
                JObject jo = new JObject();
                SwitchControl.Reset();
                jo["debug"] = 1;
                jo["verbose"] = "reset successful";
                jo["status"] = 0;
                return Response.AsText(jo.ToString(Formatting.None));
            };
            Get["/debug/trigger"] = _ =>
            {
                JObject jo = new JObject();
                SwitchControl.Trigger();
                jo["debug"] = 1;
                jo["verbose"] = "trigger successful";
                jo["status"] = 0;
                return Response.AsText(jo.ToString(Formatting.None));
            };
#endif
        }
    }
}
