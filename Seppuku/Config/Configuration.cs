using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Seppuku.Module.Config;

namespace Seppuku.Config
{
    /// <summary>
    ///     General configuration endpoint for Seppuku
    /// </summary>
    public class Configuration
    {
        private static NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        public const string ConfigurationFileName = "seppuku.json";
        private static TypeConf _instance;
        public static Dictionary<string, object> DefaultConf;

        /// <summary>
        ///     Singleton instance
        /// </summary>
        private static TypeConf I => _instance ?? (_instance = new TypeConf(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationFileName)));

        public static T Get<T>(string key, T def)=> I.Get(key, def);
        public static T Get<T>(string key) => I.Get<T>(key);
        public static void Set<T>(string key, T val){
            L.Trace("Setting configuration option {0}: {1}", key, val.ToString());
            I.Set(key, val);
        }

        public static bool Init()
        {
            DefaultConf = new Dictionary<string, object>
            {
                ["GraceTime"] = TimeSpan.FromDays(30).TotalSeconds,
                ["Port"] = 19007L,
                ["Secret"] = Guid.NewGuid().ToString()
            };
            DefaultConf["FailureDate"] = DateTime.Now.AddSeconds((double)DefaultConf["GraceTime"]);

            return I.Initialize(DefaultConf);
        }
    }
}