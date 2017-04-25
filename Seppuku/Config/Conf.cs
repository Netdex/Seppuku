using System;
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

        public static TypeConf I => _instance ??
                                                       (_instance = new TypeConf(ConfigurationFileName));
        private static TypeConf _instance;


        private static readonly Random _random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

    }
}
