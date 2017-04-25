using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Nancy;
using Nancy.Hosting.Self;
using Quartz;
using Quartz.Impl;
using Seppuku.Config;
using Seppuku.Module;
using Seppuku.Module.Utility;
using Seppuku.Switch;

namespace Seppuku
{
    class Program
    {
        static void Main(string[] args)
        {
            // show the sick greeting
            C.WriteLine(Properties.Resources.Greeting);

            #region DefaultConfiguration
            Dictionary<string, object> defaultConf = new Dictionary<string, object>()
            {
                ["GraceTime"] = XmlConvert.ToString(TimeSpan.FromDays(30)),
                ["Port"] = 19007,
                ["Secret"] = Conf.RandomString(16)
            };
            defaultConf["FailureDate"] = DateTime.Now + XmlConvert.ToTimeSpan((string)defaultConf["GraceTime"]);
            #endregion

            // load global configuration
            if (Conf.I.Initialize(defaultConf))
                C.WriteLine($"`w Configuration file did not exist or was corrupted, created {Conf.ConfigurationFileName}");
            C.WriteLine($"`i Secret key is &f{Conf.I.Conf["Secret"] as string}&r");

            // load scheduling information from global configuration
            Sched.Initialize();
            C.WriteLine($"`i Scheduled failsafe activation date at &f{(DateTime)Conf.I.Conf["FailureDate"]}&r");
            C.WriteLine($"`i Current failsafe grace delay is &f{XmlConvert.ToTimeSpan((string)Conf.I.Conf["GraceTime"])}&r");
            if (SwitchControl.Expired())
            {
                C.WriteLine($"`e Switch is already expired! No scheduling will occur.");
            }
            else
            {
                // schedule existing trigger
                Sched.ScheduleTrigger((DateTime)Conf.I.Conf["FailureDate"]);
            }
            Console.WriteLine();

            // load modules from internal and directory
            if (ModuleManager.Instance.Initialize())
            {
                C.WriteLine("`i Loaded modules: ");
                foreach (var mod in ModuleManager.Instance.TriggerModules)
                {
                    C.WriteLine($"&a{$"[{mod.Value.Name}]",20}&r - &f{mod.Value.Description}&r");
                }
            }
            else
            {
                C.WriteLine($"`e Failed to load modules from assembly location!");
                return;
            }
            Console.WriteLine();

            // run and bind all the handlers for the modules
            ModuleManager.Instance.EmitStart();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                ModuleManager.Instance.EmitStop();
            };
            Console.WriteLine();
        }
    }
}
