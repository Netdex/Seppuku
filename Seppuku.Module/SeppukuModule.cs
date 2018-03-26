using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Seppuku.Module.Config;

namespace Seppuku.Module
{

    public abstract class SeppukuModule
    {
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        public const string ModuleConfigDirectory = "Configuration";

        public string Name { get; set; }
        public string Description { get; set; }

        public string ModuleConfigPath;
        public TypeConf ModuleConfig;

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
            ModuleConfig = new TypeConf(ModuleConfigPath);
            ModuleConfig.Initialize(defaultConf);
        }


        public virtual void OnStart() { }
        public virtual void OnTrigger() { }
        public virtual void OnReset() { }
        public virtual void OnStop() { }

        public static string GetCurrentToken(string secret)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(DateTime.UtcNow.Date.ToLongDateString());
            using (HMACSHA1 myhmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(secret)))
            {
                return myhmacsha1.ComputeHash(byteArray).Aggregate("", (s, e) => s + $"{e:x2}", s => s);
            }
        }
    }
}
