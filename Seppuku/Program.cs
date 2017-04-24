using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            if(Conf.Initialize())
                C.WriteLine($"`i Configuration file did not exist or was corrupted, created {Conf.ConfigurationFileName}");

            Sched.Initialize();
            Sched.ScheduleTrigger(Conf.Configuration.FailureDate);
            C.WriteLine($"`i Scheduled failsafe activation date at {Conf.Configuration.FailureDate}");
            C.WriteLine($"`i Current failsafe grace delay is {Conf.Configuration.GraceTime}");

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
        }
    }
}
