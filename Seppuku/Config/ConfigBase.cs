using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Seppuku.Config
{
    public class ConfigBase
    {

        [XmlIgnore]
        public TimeSpan GraceTime
        {
            get => XmlConvert.ToTimeSpan(_graceTimeString);
            set => _graceTimeString = XmlConvert.ToString(value);
        }
        [Browsable(false)]
        [XmlElement("GraceTime")] 
        public string _graceTimeString { get; set; } = XmlConvert.ToString(TimeSpan.FromDays(30));

        public DateTime FailureDate { get; set; }

        public ConfigBase()
        {
            FailureDate = DateTime.Now + GraceTime;
        }
    }
}
