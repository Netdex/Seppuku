using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seppuku.Utility;

namespace Seppuku.Module.Internal
{
    [Export(typeof(TriggerModule))]
    public class ModuleAlert : TriggerModule
    {
        public ModuleAlert(): base("ModuleAlert", "Default test module")
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
