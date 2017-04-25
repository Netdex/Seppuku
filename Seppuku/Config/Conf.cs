using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Seppuku.Module;
using Seppuku.Module.Config;

namespace Seppuku.Config
{
    /// <summary>
    /// General configuration endpoint for Seppuku
    /// </summary>
    class Conf
    {
        public const string ConfigurationFileName = "seppuku.xml";

        private static TypeConf I => _instance ?? (_instance = new TypeConf(ConfigurationFileName));
        private static TypeConf _instance;

        public static T Get<T> (string key, T def) => _instance.Conf.ContainsKey(key) ? (T)_instance.Conf[key] : def;
        public static void Set<T>(string key, T val)
        {
            _instance.Conf[key] = val;
            I.Save();
        }

        public static bool Init()
        {
            Dictionary<string, object> defaultConf = new Dictionary<string, object>()
            {
                ["GraceTime"] = TimeSpan.FromSeconds(15).TotalSeconds,
                ["Port"] = 19007,
                ["Secret"] = RandomString(16)
            };
            defaultConf["FailureDate"] = DateTime.Now.AddSeconds((double)defaultConf["GraceTime"]);

            return I.Initialize(defaultConf);
        }

        private static readonly Random _random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

    }
}
