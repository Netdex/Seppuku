using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Seppuku.Module.Config
{
    /// <summary>
    /// A hack I copied verbatim from stackoverflow to make serializing Dictionaries possible
    /// </summary>
    public class DictItem
    {
        [XmlAttribute]
        public string ID;
        [XmlElement]
        public object Value;
    }

    /// <summary>
    /// A glorified Dictionary to file serialization for configuration
    /// </summary>
    public class TypeConf
    {
        public string ConfigurationFileName { get; set; }

        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(DictItem[]),
            new XmlRootAttribute() { ElementName = "items" });
        public Dictionary<string, object> Conf;

        public TypeConf(string file)
        {
            ConfigurationFileName = file;
        }

        public bool Initialize(Dictionary<string, object> defaults)
        {
            bool stat = Load();
            if (!stat)
            {
                Conf = new Dictionary<string, object>(defaults);
                Save();
            }
            return !stat;
        }

        public bool Load()
        {
            try
            {
                var strm = File.OpenRead(ConfigurationFileName);
                Conf = ((DictItem[])_serializer.Deserialize(strm))
                    .ToDictionary(i => i.ID, i => i.Value);
                strm.Close();
                return Conf != null;
            }
            catch
            {
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                var strm = File.OpenWrite(ConfigurationFileName);
                _serializer.Serialize(strm, Conf.Select(kv => new DictItem() { ID = kv.Key, Value = kv.Value }).ToArray());
                strm.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
