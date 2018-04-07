using System.Reflection;
using Nancy;
using Nancy.Extensions;
using Nancy.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seppuku.Module.Standard;
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
            // Root directory, for polling program information
            Get["/"] = _ =>
            {
                var jo = new JObject
                {
                    ["application"] = Assembly.GetExecutingAssembly().GetName().Name,
                    ["version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    ["description"] = "automated digital dead man's switch and executor"
                };
                return Response.AsText(jo.ToString(Formatting.None));
            };

            /*
             * If expired,      return status=Expired, remaining=0
             * If not,          return status=Pending, remaining=time
             */
            Get["/remain"] = _ =>
            {
                var timeleft = SwitchControl.TimeLeft();
                var jo = new JObject();
                if (SwitchControl.IsExpired)
                {
                    jo["status"] = (int)StatusCode.Expired;
                    jo["verbose"] = "expired";
                    jo["remaining"] = 0;
                }
                else
                {
                    jo["status"] = (int) StatusCode.Success;
                    jo["verbose"] = timeleft.ToString() + " remaining";
                    jo["remaining"] = timeleft.TotalSeconds;
                }
                return Response.AsText(jo.ToString(Formatting.None));
            };
            // TODO I did not finish migrating to status codes, finish that
            /*
             * If unauthorized, return status=Unauthorized
             * If expired,      return status=Expired
             * Return status=Success
             */
            Get["/reset/{token}"] = param =>
            {
                var jo = new JObject();
                if (SwitchControl.IsAuthorized(param.token))
                {
                    if (SwitchControl.IsExpired)
                    {
                        jo["verbose"] = "cannot reset expired timer";
                        jo["status"] = (int) StatusCode.Expired;
                    }
                    else
                    {
                        SwitchControl.Reset();
                        jo["verbose"] = "reset successful";
                        jo["status"] = (int) StatusCode.Success;
                    }
                }
                else
                {
                    jo["verbose"] = "unauthorized";
                    jo["status"] = (int) StatusCode.Unauthorized;
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
                jo["status"] = (int) StatusCode.Success;
                return Response.AsText(jo.ToString(Formatting.None));
            };
            Get["/debug/trigger"] = _ =>
            {
                var jo = new JObject();
                SwitchControl.Trigger();
                jo["debug"] = 1;
                jo["verbose"] = "trigger successful";
                jo["status"] = (int) StatusCode.Success;
                return Response.AsText(jo.ToString(Formatting.None));
            };
#endif
        }
    }
}
 