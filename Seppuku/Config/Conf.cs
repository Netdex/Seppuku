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
    internal class Conf
    {
        public const string ConfigurationFileName = "seppuku.json";
        private static TypeConf _instance;
        public static Dictionary<string, object> DefaultConf;

        private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

        /// <summary>
        ///     Singleton instance
        /// </summary>
        private static TypeConf I => _instance ?? (_instance = new TypeConf(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationFileName)));

        public static T Get<T>(string key, T def)=> I.Get(key, def);
        public static T Get<T>(string key, Dictionary<string, object> defaults) => I.Get<T>(key, defaults);
        public static void Set<T>(string key, T val)=> I.Set(key, val);

        public static bool Init()
        {
            DefaultConf = new Dictionary<string, object>
            {
                ["GraceTime"] = TimeSpan.FromDays(30).TotalSeconds,
                ["Port"] = 19007L,
                ["Secret"] = RandomString(16)
            };
            DefaultConf["FailureDate"] = DateTime.Now.AddSeconds((double)DefaultConf["GraceTime"]);

            return I.Initialize(DefaultConf);
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