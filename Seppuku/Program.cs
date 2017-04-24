using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Hosting.Self;
using Quartz;
using Quartz.Impl;
using Seppuku.Module;
using Seppuku.Switch;
using Seppuku.Utility;

namespace Seppuku
{
    class Program
    {
        static void Main(string[] args)
        {
            C.WriteLine(Properties.Resources.Greeting);

            if (Conf.Initialize())
                C.WriteLine($"`i Configuration file did not exist or was corrupted, created {Conf.ConfigurationFileName}");

            Sched.Initialize();
            
            C.WriteLine($"`i Scheduled failsafe activation date at {Conf.Configuration.FailureDate}");
            C.WriteLine($"`i Current failsafe grace delay is {Conf.Configuration.GraceTime}");
            if (SwitchControl.Expired())
            {
                C.WriteLine($"`e Switch is already expired! No scheduling will occur.");
            }
            else
            {
                Sched.ScheduleTrigger(Conf.Configuration.FailureDate);
            }
            Console.WriteLine();

            if (ModuleManager.Instance.Initialize())
            {
                C.WriteLine("`i Loaded modules: ");
                foreach (var mod in ModuleManager.Instance.TriggerModules)
                {
                    C.WriteLine($"\t{mod.Value.Name} - {mod.Value.Description}");
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
            }, new Uri($"http://localhost:{Conf.Configuration.Port}")))
            {
                host.Start();
                C.WriteLine($"`i Running on http://localhost:{Conf.Configuration.Port}/");
                Console.ReadLine();
            }

            C.WriteLine("`w Local server terminated! No commands will work!");

        }
    }
}
