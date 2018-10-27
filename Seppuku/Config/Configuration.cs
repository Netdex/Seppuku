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

        private static readonly TypeConf Instance =
            new TypeConf(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                ConfigurationFileName));
        public static Dictionary<string, object> DefaultConf;


        public static T Get<T>(string key, T def)=> Instance.Get(key, def);
        public static T Get<T>(string key) => Instance.Get<T>(key);
        public static void Set<T>(string key, T val){
            L.Trace("Setting configuration option {0}: {1}", key, val.ToString());
            Instance.Set(key, val);
        }

        public static bool Init()
        {
            DefaultConf = new Dictionary<string, object>
            {
                ["GraceTime"] = TimeSpan.FromDays(30).TotalSeconds,
                ["Secret"] = Guid.NewGuid().ToString()
            };
            DefaultConf["FailureDate"] = DateTime.Now.AddSeconds((double)DefaultConf["GraceTime"]);

            return Instance.Initialize(DefaultConf);
        }
    }
}