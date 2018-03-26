using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace Seppuku.Module.Internal.ModuleFileFlag
{
    /// <summary>
    ///     Maintains a flag in the form of a file that other programs can access
    /// </summary>
    [Export(typeof(SeppukuModule))]
    public class ModuleFileFlag : SeppukuModule
    {
        public ModuleFileFlag() : base("ModuleFileFlag", "Creates file flags for other programs to look at", new Dictionary<string, object>()
        {
            ["RunningFlagFileName"] = "running",
            ["TriggerFlagFileName"] = "trigger"
        })
        {
        }

        public override void OnStart()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.Create(Path.Combine(cd, ModuleConfig.Get<string>("RunningFlagFileName"))).Close();
        }

        public override void OnTrigger()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.Create(Path.Combine(cd, ModuleConfig.Get<string>("TriggerFlagFileName"))).Close();
        }

        public override void OnReset()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.Delete(Path.Combine(cd, ModuleConfig.Get<string>("TriggerFlagFileName")));
        }

        public override void OnStop()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.Delete(Path.Combine(cd, ModuleConfig.Get<string>("RunningFlagFileName")));
        }
    }
}
