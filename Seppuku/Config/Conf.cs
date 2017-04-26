using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        public const string ConfigurationFileName = "seppuku.json";

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static TypeConf I => _instance ?? (_instance = new TypeConf(ConfigurationFileName));
        private static TypeConf _instance;

        public static T Get<T> (string key, T def) => I.Get(key ,def);
        public static void Set<T>(string key, T val) => I.Set(key, val);

        public static bool Init()
        {
            Dictionary<string, object> defaultConf = new Dictionary<string, object>()
            {
                ["GraceTime"] = TimeSpan.FromSeconds(15).TotalSeconds,
                ["Port"] = 19007L,
                ["Secret"] = RandomString(16)
            };
            defaultConf["FailureDate"] = DateTime.Now.AddSeconds((double)defaultConf["GraceTime"]);

            return I.Initialize(defaultConf);
        }

        private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            byte[] rand = new byte[length];
            Random.GetBytes(rand);
            char[] data = new char[length];
            for (int i = 0; i < length; i++)
                data[i] = chars[rand[i] % chars.Length];
            return new string(data);
        }

    }
}
