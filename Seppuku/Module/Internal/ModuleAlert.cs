using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seppuku.Module.Internal
{
    [Export(typeof(TriggerModule))]
    public class ModuleAlert : TriggerModule
    {
        private static Dictionary<string, object> DefaultConf = new Dictionary<string, object>();

        public ModuleAlert(): base("ModuleAlert", "Default test module", DefaultConf)
        {

        }

        public override void Start()
        {
            Log("Modules loaded");
        }

        public override void Trigger()
        {
            Log("Deadman switch triggered");
        }

        public override void Stop()
        {
            Log("Modules stopped");
        }
    }
}
