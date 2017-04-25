using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Seppuku.Config;

namespace Seppuku.Switch
{
    /// <summary>
    /// Defines most helper methods to manage the deadman's switch
    /// </summary>
    class SwitchControl
    {
        public static void Reset()
        {
            Conf.I.Conf["FailureDate"] = DateTime.Now + XmlConvert.ToTimeSpan((string)Conf.I.Conf["GraceTime"]);
            Conf.I.Save();
            Sched.UnscheduleTrigger();
            Sched.ScheduleTrigger((DateTime) Conf.I.Conf["FailureDate"]);
        }

        public static TimeSpan TimeLeft()
        {
            return (DateTime)Conf.I.Conf["FailureDate"] - DateTime.Now;
        }

        public static bool Expired()
        {
            return TimeLeft() < TimeSpan.Zero;
        }

        public static bool Authorized(string passphrase)
        {
            return passphrase == (string)Conf.I.Conf["Secret"];
        }
    }
}
