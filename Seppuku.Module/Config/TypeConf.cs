using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Seppuku.Module.Config
{
    /// <summary>
    ///     A glorified Dictionary to file serialization for configuration
    /// </summary>
    public class TypeConf
    {
        public Dictionary<string, object> Conf;
        public Dictionary<string, object> Defaults;

        public TypeConf(string file)
        {
            ConfigurationFileName = file;
        }

        public string ConfigurationFileName { get; set; }

        public T Get<T>(string key, T def)
        {
            return Conf.ContainsKey(key) ? (T) Conf[key] : def;
        }

        public T Get<T>(string key)
        {
            return Get(key, (T)Defaults[key]);
        }

        public void Set<T>(string key, T val)
        {
            Conf[key] = val;
            Save();
        }

        public bool Initialize(Dictionary<string, object> defaults)
        {
            Defaults = new Dictionary<string, object>(defaults);

            var stat = Load();
            if (!stat)
            {
                Conf = new Dictionary<string, object>(defaults);
                Save();
            }
            return !stat;
        }

        public bool Load()
        {
            FileStream strm = null;
            try
            {
                strm = File.Open(ConfigurationFileName, FileMode.Open, FileAccess.Read);
                using (var sr = new StreamReader(strm))
                {
                    Conf = JsonConvert.DeserializeObject<Dictionary<string, object>>(sr.ReadToEnd(),
                        new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});
                }
                strm.Close();
                return Conf != null;
            }
            catch
            {
                strm?.Close();
                return false;
            }
        }

        public bool Save()
        {
            FileStream strm = null;
            try
            {
                strm = File.Open(ConfigurationFileName, FileMode.Create, FileAccess.Write);
                using (var sw = new StreamWriter(strm))
                {
                    sw.Write(JsonConvert.SerializeObject(Conf, typeof(object),
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                            Formatting = Formatting.Indented
                        }));
                }
                strm.Close();
                return true;
            }
            catch
            {
                strm?.Close();
                return false;
            }
        }
    }
}