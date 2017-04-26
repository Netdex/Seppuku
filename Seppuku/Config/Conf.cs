using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Seppuku.Module.Config;

namespace Seppuku.Config
{
    /// <summary>
    ///     General configuration endpoint for Seppuku
    /// </summary>
    internal class Conf
    {
        public const string ConfigurationFileName = "seppuku.json";
        private static TypeConf _instance;

        private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

        /// <summary>
        ///     Singleton instance
        /// </summary>
        private static TypeConf I => _instance ?? (_instance = new TypeConf(ConfigurationFileName));

        public static T Get<T>(string key, T def)
        {
            return I.Get(key, def);
        }

        public static void Set<T>(string key, T val)
        {
            I.Set(key, val);
        }

        public static bool Init()
        {
            var defaultConf = new Dictionary<string, object>
            {
                ["GraceTime"] = TimeSpan.FromDays(30).TotalSeconds,
                ["Port"] = 19007L,
                ["Secret"] = RandomString(16)
            };
            defaultConf["FailureDate"] = DateTime.Now.AddSeconds((double) defaultConf["GraceTime"]);

            return I.Initialize(defaultConf);
        }

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rand = new byte[length];
            Random.GetBytes(rand);
            var data = new char[length];
            for (var i = 0; i < length; i++)
                data[i] = chars[rand[i] % chars.Length];
            return new string(data);
        }
    }
}