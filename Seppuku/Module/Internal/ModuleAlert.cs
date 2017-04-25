using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seppuku.Module.Internal
{
    /// <summary>
    /// Simple demonstration module which prints a message on every event
    /// </summary>
    [Export(typeof(TriggerModule))]
    public class ModuleAlert : TriggerModule
    {
        private static Dictionary<string, object> DefaultConf = new Dictionary<string, object>();

        public ModuleAlert(): base("ModuleAlert", "Default test module", DefaultConf)
        {

        }

        public override void OnStart()
        {
            Log("Modules loaded");
        }

        public override void OnTrigger()
        {
            Log("Deadman switch triggered");
        }

        public override void OnStop()
        {
            Log("Modules stopped");
        }
    }
}
