﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Seppuku.Module.Internal
{
    /// <summary>
    ///     Maintains a flag in the form of a file that other programs can access
    /// </summary>
    [Export(typeof(SeppukuModule))]
    public class ModuleFileFlag : SeppukuModule
    {
        private static readonly Dictionary<string, object> DefaultConf = new Dictionary<string, object>()
        {
            ["RunningFlagFileName"] = "running",
            ["TriggerFlagFileName"] = "trigger"
        };

        public ModuleFileFlag() : base("ModuleFileFlag", "Creates file flags for other programs to look at", DefaultConf)
        {
        }

        public override void OnStart()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.Create(Path.Combine(cd, Configuration.Get<string>("RunningFlagFileName", DefaultConf)));
        }

        public override void OnTrigger()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.Create(Path.Combine(cd, Configuration.Get<string>("TriggerFlagFileName", DefaultConf)));
        }

        public override void OnReset()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.Delete(Path.Combine(cd, Configuration.Get<string>("TriggerFlagFileName", DefaultConf)));
        }

        public override void OnStop()
        {
            string cd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.Delete(Path.Combine(cd, Configuration.Get<string>("RunningFlagFileName", DefaultConf)));
        }
    }
}
