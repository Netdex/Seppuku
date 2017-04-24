using System.IO;
using System.Xml.Serialization;

namespace Seppuku.Config
{
    class Conf
    {
        public const string ConfigurationFileName = "seppuku.xml";

        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ConfigBase));
        public static ConfigBase Configuration;

        public static bool Initialize()
        {
            bool stat = Load();
            if (!stat)
            {
                Configuration = new ConfigBase();
                Save();
            }
            return !stat;
        }

        public static bool Load()
        {
            try
            {
                Configuration = (ConfigBase)Serializer.Deserialize(File.OpenRead(ConfigurationFileName));
                return Configuration != null;
            }
            catch
            {
                return false;
            }
        }

        public static bool Save()
        {
            try
            {
                Serializer.Serialize(File.OpenWrite(ConfigurationFileName), Configuration);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
