using System;
using Seppuku.Config;
using Seppuku.Module;
using Seppuku.Properties;
using Seppuku.Switch;

namespace Seppuku
{
    internal class Program
    {
        private static NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();
        private static void Main(string[] args)
        {
            // show the sick greeting
            //Console.WriteLine(Resources.Greeting);

            // load global configuration
            if (Configuration.Init())
                L.Warn("Configuration file did not exist or was corrupted, created {0}",
                    Configuration.ConfigurationFileName);
            L.Info("Secret key is {0}",
                Configuration.Get<string>("Secret"));
            L.Info("Today's auth token is {0}", SeppukuModule.GetCurrentToken(Configuration.Get<string>("Secret")));

            // load scheduling information from global configuration
            Sched.Initialize();
            L.Info("Scheduled failsafe activation date at {0}", Configuration.Get<DateTime>("FailureDate"));
            L.Info("Current failsafe grace delay is {0}",
                TimeSpan.FromSeconds(Configuration.Get<double>("GraceTime")));

            // load modules from internal and directory
            if (ModuleManager.Init())
            {
                L.Info("Loaded modules: ");
                foreach (var mod in ModuleManager.Modules)
                    L.Info("{0} {1}", $"[{mod.Value.Name}]", mod.Value.Description);
            }
            else
            {
                L.Error("Failed to load modules from assembly location!");
                return;
            }

            if (SwitchControl.IsTriggered)
            {
                L.Warn("Switch has been triggered already! Will not trigger again.");
            }
            else
            {
                if (SwitchControl.IsExpired)
                {
                    L.Warn("Expiry date passed, but trigger has not occurred yet! Triggering now.");
                    SwitchControl.Trigger();
                }
            }

            if (SwitchControl.IsTriggered)
            {
                L.Warn("Switch is already expired! No scheduling will occur.");
            }
            else
            {
                Sched.ScheduleTrigger(Configuration.Get<DateTime>("FailureDate"));
            }

            // run and bind all the handlers for the modules
            ModuleManager.Emit(EmitType.Start);
            Console.CancelKeyPress += (sender, eventArgs) => { ModuleManager.Emit(EmitType.Stop); };
        }
    }
}