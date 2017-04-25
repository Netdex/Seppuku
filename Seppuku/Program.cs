using System;
using System.Collections.Generic;
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
            C.WriteLine(Properties.Resources.Greeting);

            #region DefaultConfiguration
            Dictionary<string, object> defaultConf = new Dictionary<string, object>()
            {
                ["GraceTime"] = XmlConvert.ToString(TimeSpan.FromDays(30)),
                ["Port"] = 19007,
                ["Secret"] = Conf.RandomString(16)
            };
            defaultConf["FailureDate"] = DateTime.Now + XmlConvert.ToTimeSpan((string) defaultConf["GraceTime"]);
            #endregion

            if (Conf.Instance.Initialize(defaultConf))
                C.WriteLine($"`w Configuration file did not exist or was corrupted, created {Conf.ConfigurationFileName}");

            C.WriteLine($"`i Secret key is {Conf.Instance.Configuration["Secret"] as string}");
            Sched.Initialize();
            C.WriteLine($"`i Scheduled failsafe activation date at {(DateTime)Conf.Instance.Configuration["FailureDate"]}");
            C.WriteLine($"`i Current failsafe grace delay is {(string) Conf.Instance.Configuration["GraceTime"]}");
            if (SwitchControl.Expired())
            {
                C.WriteLine($"`e Switch is already expired! No scheduling will occur.");
            }
            else
            {
                // schedule existing trigger
                Sched.ScheduleTrigger((DateTime) Conf.Instance.Configuration["FailureDate"] );
            }
            Console.WriteLine();

            if (ModuleManager.Instance.Initialize())
            {
                C.WriteLine("`i Loaded modules: ");
                foreach (var mod in ModuleManager.Instance.TriggerModules)
                {
                    C.WriteLine($"\t&a{mod.Value.Name}&r - &f{mod.Value.Description}&r");
                }
            }
            else
            {
                C.WriteLine($"`e Failed to load modules from assembly location!");
                return;
            }
            Console.WriteLine();

            ModuleManager.Instance.EmitStart();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                ModuleManager.Instance.EmitStop();
            };
            Console.WriteLine();

            C.WriteLine("`i Running local server for command execution");
            using (var host = new NancyHost(new DefaultNancyBootstrapper(), new HostConfiguration
            {
                RewriteLocalhost = false
            }, new Uri($"http://localhost:{Conf.Instance.Configuration["Port"] as int?}")))
            {
                host.Start();
                C.WriteLine($"`i Running on http://localhost:{Conf.Instance.Configuration["Port"] as int?}/");
                Console.WriteLine();
                Console.ReadLine();
            }
           

            C.WriteLine("`w Local server terminated! No commands will work!");

        }
    }
}
