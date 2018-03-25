using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Seppuku.Module.Internal.ModuleAlert
{
    /// <summary>
    ///     Simple demonstration module which prints a message on every event
    /// </summary>
    [Export(typeof(SeppukuModule))]
    public class ModuleAlert : SeppukuModule
    {
        public ModuleAlert() : base("ModuleAlert", "Default test module", new Dictionary<string, object>())
        {
        }

        public override void OnStart()
        {
            Log("Modules loaded");
        }

        public override void OnTrigger()
        {
            Log("Deadman's switch triggered");
        }

        public override void OnReset()
        {
            Log("Reset, new trigger date is {0}", Seppuku.Config.Configuration.Get<DateTime>("FailureDate", Seppuku.Config.Configuration.DefaultConf));
        }

        public override void OnStop()
        {
            Log("Modules stopped");
        }
    }
}