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

            // load global configuration
            if (Conf.Init())
                C.WriteLine("`w Configuration file did not exist or was corrupted, created {0}", Conf.ConfigurationFileName);
            C.WriteLine("`i Secret key is &f{0}&r, today's auth token is &f{1}&r",
                Conf.Get<string>("Secret", null), SeppukuAuth.GetCurrentToken(Conf.Get<string>("Secret", null)));

            // load scheduling information from global configuration
            Sched.Initialize();
            C.WriteLine("`i Scheduled failsafe activation date at &f{0}&r", Conf.Get<DateTime?>("FailureDate", null));
            C.WriteLine("`i Current failsafe grace delay is &f{0}&r", TimeSpan.FromSeconds(Conf.Get("GraceTime", Double.MinValue)));
            if (SwitchControl.Triggered())
            {
                C.WriteLine("`e Switch is already expired! No scheduling will occur.");
            }
            else
            {
                // schedule existing trigger
                Sched.ScheduleTrigger(Conf.Get("FailureDate", DateTime.MaxValue));
            }
            Console.WriteLine();

            // load modules from internal and directory
            if (ModuleManager.Init())
            {
                C.WriteLine("`i Loaded modules: ");
                foreach (var mod in ModuleManager.Modules)
                {
                    C.WriteLine("&a{0,20}&r - &f{1}&r", $"[{mod.Value.Name}]", mod.Value.Description);
                }
            }
            else
            {
                C.WriteLine("`e Failed to load modules from assembly location!");
                return;
            }
            Console.WriteLine();

            // run and bind all the handlers for the modules
            ModuleManager.Emit(EmitType.Start);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                ModuleManager.Emit(EmitType.Stop);
            };
            Console.WriteLine();
        }
    }
}
