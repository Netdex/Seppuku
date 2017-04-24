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
        #region UserVariables
        /// <summary>
        /// Grace period after a deadman switch reset
        /// All this funky stuff is because you can't serialize a TimeSpan smh
        /// </summary>
        [XmlIgnore]
        public TimeSpan GraceTime
        {
            get => XmlConvert.ToTimeSpan(_graceTimeString);
            set => _graceTimeString = XmlConvert.ToString(value);
        }
        [Browsable(false)]
        [XmlElement("GraceTime")]
        public string _graceTimeString { get; set; } = XmlConvert.ToString(TimeSpan.FromDays(30));

        /// <summary>
        /// Listening port of the web-based interface
        /// </summary>
        public int Port { get; set; } = 19007;

        /// <summary>
        /// Secret for performing authorized operations
        /// </summary>
        public string Secret { get; set; } = RandomString(16);
        #endregion

        /* internal variables are used by the program to track internal state,
         * and should not be modified in the user configuration
         */
        #region InternalVariables
        /// <summary>
        /// Date in the future (GraceTime + DateTime.Now) where the switch is triggered
        /// </summary>
        public DateTime FailureDate { get; set; }
        #endregion

        public ConfigBase()
        {
            FailureDate = DateTime.Now + GraceTime;
        }


        private static readonly Random random = new Random();
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
