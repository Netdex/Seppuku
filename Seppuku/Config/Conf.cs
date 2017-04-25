using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Seppuku.Module;
using Seppuku.Module.Config;

namespace Seppuku.Config
{
    class Conf
    {
        public const string ConfigurationFileName = "seppuku.xml";

        public static TypeConf Instance => _instance ??
                                                       (_instance = new TypeConf(ConfigurationFileName));
        private static TypeConf _instance;


        private static readonly Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
