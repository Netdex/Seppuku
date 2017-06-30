using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seppuku.Module.Honest.Backend
{
    [Export(typeof(SeppukuModule))]
    public class ModuleHonestBackend : SeppukuModule
    {
        public ModuleHonestBackend() : base(
            "Honest Backend",
            "Backend module for Brutal Honesty Delivery System",
            new Dictionary<string, object>()
            {

            })
        {
        }

        public override void OnStart()
        {

        }

        public override void OnTrigger()
        {

        }

        public override void OnReset()
        {

        }

        public override void OnStop()
        {

        }
    }
}
