using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Seppuku.Module.Config
{
    public class DictItem
    {
        [XmlAttribute]
        public string ID;
        [XmlElement]
        public object Value;
    }

    public class TypeConf
    {
        public string ConfigurationFileName { get; set; }

        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(DictItem[]),
            new XmlRootAttribute() { ElementName = "items" });
        public Dictionary<string, object> Configuration;

        public TypeConf(string file)
        {
            ConfigurationFileName = file;
        }

        public bool Initialize(Dictionary<string, object> defaults)
        {
            bool stat = Load();
            if (!stat)
            {
                Configuration = new Dictionary<string, object>(defaults);
                Save();
            }
            return !stat;
        }

        public bool Load()
        {
            try
            {
                var strm = File.OpenRead(ConfigurationFileName);
                Configuration = ((DictItem[])_serializer.Deserialize(strm))
                    .ToDictionary(i => i.ID, i => i.Value);
                strm.Close();
                return Configuration != null;
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
                _serializer.Serialize(strm, Configuration.Select(kv => new DictItem() { ID = kv.Key, Value = kv.Value }).ToArray());
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
