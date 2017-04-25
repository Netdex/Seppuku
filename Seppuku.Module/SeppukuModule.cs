using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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


        protected SeppukuModule(string name, string description, Dictionary<string, object> defaultConf) 
        {
            Name = name;
            Description = description;

            var confPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                ModuleConfigDirectory);
            Directory.CreateDirectory(confPath);
            ModuleConfigPath = Path.Combine(confPath, GetType().FullName + ".xml");
            Configuration = new TypeConf(ModuleConfigPath);
            Configuration.Initialize(defaultConf);
        }
        

        public abstract void OnStart();
        public abstract void OnTrigger();
        public abstract void OnStop();

        public void Log(string s)
        {
            C.WriteLine($"&a{$"[{Name}]",20} &f{s}&r");
        }
    }
}
