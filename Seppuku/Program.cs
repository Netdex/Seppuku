using System;
using Seppuku.Config;
using Seppuku.Module;
using Seppuku.Module.Utility;
using Seppuku.Properties;
using Seppuku.Switch;

namespace Seppuku
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // show the sick greeting
            C.WriteLine(Resources.Greeting);

            // load global configuration
            if (Configuration.Init())
                C.WriteLine("`w Configuration file did not exist or was corrupted, created {0}",
                    Configuration.ConfigurationFileName);
            C.WriteLine("`i Secret key is &f{0}&r, today's auth token is &f{1}&r",
                Configuration.Get<string>("Secret", Configuration.DefaultConf), SeppukuAuth.GetCurrentToken(Configuration.Get<string>("Secret", Configuration.DefaultConf)));

            // load scheduling information from global configuration
            Sched.Initialize();
            C.WriteLine("`i Scheduled failsafe activation date at &f{0}&r", Configuration.Get<DateTime>("FailureDate", Configuration.DefaultConf));
            C.WriteLine("`i Current failsafe grace delay is &f{0}&r",
                TimeSpan.FromSeconds(Configuration.Get<double>("GraceTime", Configuration.DefaultConf)));
            Console.WriteLine();

            // load modules from internal and directory
            if (ModuleManager.Init())
            {
                C.WriteLine("`i Loaded modules: ");
                foreach (var mod in ModuleManager.Modules)
                    C.WriteLine("&a{0,20}&r - &f{1}&r", $"[{mod.Value.Name}]", mod.Value.Description);
            }
            else
            {
                C.WriteLine("`e Failed to load modules from assembly location!");
                return;
            }

            Console.WriteLine();
            
            if (SwitchControl.IsExpired)
                SwitchControl.Trigger();

            if (SwitchControl.IsTriggered)
                C.WriteLine("`e Switch is already expired! No scheduling will occur.");
            else
                Sched.ScheduleTrigger(Configuration.Get<DateTime>("FailureDate", Configuration.DefaultConf));
            Console.WriteLine();

            // run and bind all the handlers for the modules
            ModuleManager.Emit(EmitType.Start);
            Console.CancelKeyPress += (sender, eventArgs) => { ModuleManager.Emit(EmitType.Stop); };
            Console.WriteLine();
        }
    }
}