using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Seppuku.Config;

namespace Seppuku.Module.Internal.ModuleAlert
{
    /// <summary>
    ///     Simple demonstration module which prints a message on every event
    /// </summary>
    [Export(typeof(SeppukuModule))]
    public class ModuleAlert : SeppukuModule
    {
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        public ModuleAlert() : base("ModuleAlert", "Default test module", new Dictionary<string, object>())
        {
        }

        public override void OnStart()
        {
            L.Warn("Modules loaded");
        }

        public override void OnTrigger()
        {
            L.Warn("Deadman's switch triggered");
        }

        public override void OnReset()
        {
            L.Warn("Reset, new trigger date is {0}", Configuration.Get<DateTime>("FailureDate"));
        }

        public override void OnStop()
        {
            L.Warn("Modules stopped");
        }
    }
}