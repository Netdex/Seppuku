using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Seppuku.Module.Config;
using Seppuku.Module.Utility;

namespace Seppuku.Module
{
    public abstract class SeppukuModule
    {
        public const string ModuleConfigDirectory = "Configuration";

        public string Name { get; set; }
        public string Description { get; set; }

        public string ModuleConfigPath;
        public TypeConf Configuration;

        public readonly Dictionary<string, object> DefaultConf;

        protected SeppukuModule(string name, string description, Dictionary<string, object> defaultConf) 
        {
            Name = name;
            Description = description;
            DefaultConf = defaultConf;

            var confPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                ModuleConfigDirectory);
            Directory.CreateDirectory(confPath);
            ModuleConfigPath = Path.Combine(confPath, GetType().FullName + ".json");
            Configuration = new TypeConf(ModuleConfigPath);
            Configuration.Initialize(defaultConf);
        }
        

        public virtual void OnStart() { }
        public virtual void OnTrigger() { }
        public virtual void OnReset() { }
        public virtual void OnStop() { }


        [StringFormatMethod("format")]
        public void Log(string format, params object[] param)
        {
            format = string.Format(format, param);
            try
            {
                C.WriteLine("&7<{0}>&r &a{1,20} &f{2}&r", DateTime.Now, $"[{Name}]".Truncate(20), format);
            }
            catch
            {
                C.WriteLine("`e Invalid formatting in message from {0}", Name);
            }
        }
    }
}
