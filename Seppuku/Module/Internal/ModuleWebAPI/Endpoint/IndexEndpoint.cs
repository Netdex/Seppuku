using System.Reflection;
using Nancy;
using Nancy.Extensions;
using Nancy.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seppuku.Switch;

namespace Seppuku.Module.Internal.ModuleWebAPI.Endpoint
{
    /// <summary>
    ///     Defines all web api endpoints
    /// </summary>
    public class IndexEndpoint : NancyModule
    {
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        public IndexEndpoint()
        {
            Get["/"] = _ =>
            {
                var jo = new JObject
                {
                    ["application"] = Assembly.GetExecutingAssembly().GetName().Name,
                    ["version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    ["description"] = "automated digital deadman's switch and executor"
                };
                return Response.AsText(jo.ToString(Formatting.None));
            };

            Get["/remain"] = _ =>
            {
                var timeleft = SwitchControl.TimeLeft();
                var jo = new JObject();
                if (SwitchControl.IsExpired)
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

            Get["/reset/{token}"] = param =>
            {
                var jo = new JObject();
                if (SwitchControl.IsAuthorized(param.token))
                {
                    if (SwitchControl.IsExpired)
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

            Get["/trigger/{token}"] = param =>
            {
                var jo = new JObject();
                if (SwitchControl.IsAuthorized(param.token))
                {
                    if (SwitchControl.IsExpired)
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
                var jo = new JObject();
                SwitchControl.Reset();
                jo["debug"] = 1;
                jo["verbose"] = "reset successful";
                jo["status"] = 0;
                return Response.AsText(jo.ToString(Formatting.None));
            };
            Get["/debug/trigger"] = _ =>
            {
                var jo = new JObject();
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