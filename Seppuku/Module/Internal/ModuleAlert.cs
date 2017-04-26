using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Seppuku.Module.Internal
{
    /// <summary>
    ///     Simple demonstration module which prints a message on every event
    /// </summary>
    [Export(typeof(SeppukuModule))]
    public class ModuleAlert : SeppukuModule
    {
        private static readonly Dictionary<string, object> DefaultConf = new Dictionary<string, object>();

        public ModuleAlert() : base("ModuleAlert", "Default test module", DefaultConf)
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
            Log("Deadman's switch reset");
        }

        public override void OnStop()
        {
            Log("Modules stopped");
        }
    }
}