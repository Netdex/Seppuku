using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace Seppuku.Module.FileFlag
{
    /// <summary>
    ///     Maintains a flag in the form of a file that other programs can access
    /// </summary>
    [Export(typeof(SeppukuModule))]
    public class ModuleFileFlag : SeppukuModule
    {
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();
        
        public ModuleFileFlag() : base("ModuleFileFlag", "Creates file flags for other programs to look at", new Dictionary<string, object>()
        {
            ["RunningFlagFileName"] = "running",
            ["TriggerFlagFileName"] = "trigger"
        })
        {
        }

        public override void OnStart(bool triggered)
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.Create(Path.Combine(cd, ModuleConfig.Get<string>("RunningFlagFileName"))).Close();
            if (triggered)
            {
                OnTrigger();
            }
            else
            {
                OnReset();
            }
        }

        public override void OnTrigger()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                File.Create(Path.Combine(cd, ModuleConfig.Get<string>("TriggerFlagFileName"))).Close();
            }
            catch (IOException ex)
            {
                L.Trace(ex, "file trigger error");
            }
        }

        public override void OnReset()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                File.Delete(Path.Combine(cd, ModuleConfig.Get<string>("TriggerFlagFileName")));
            }
            catch (IOException ex)
            {
                L.Trace(ex, "file reset error");
            }
        }

        public override void OnStop()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                File.Delete(Path.Combine(cd, ModuleConfig.Get<string>("RunningFlagFileName")));
            }
            catch (IOException ex)
            {
                L.Trace(ex, "file stop error");
            }
        }
    }
}
