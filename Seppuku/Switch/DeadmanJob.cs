using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Seppuku.Module;

namespace Seppuku.Switch
{
    class DeadmanJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ModuleManager.Instance.EmitTrigger();
        }
    }
}
