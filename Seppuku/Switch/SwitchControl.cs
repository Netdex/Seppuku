using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Seppuku.Config;

namespace Seppuku.Switch
{
    class SwitchControl
    {
        public static void Reset()
        {
            Conf.Instance.Configuration["FailureDate"] = DateTime.Now + XmlConvert.ToTimeSpan((string)Conf.Instance.Configuration["GraceTime"]);
            Conf.Instance.Save();
            Sched.UnscheduleTrigger();
            Sched.ScheduleTrigger((DateTime) Conf.Instance.Configuration["FailureDate"]);
        }

        public static TimeSpan TimeLeft()
        {
            return (DateTime)Conf.Instance.Configuration["FailureDate"] - DateTime.Now;
        }

        public static bool Expired()
        {
            return TimeLeft() < TimeSpan.Zero;
        }

        public static bool Authorized(string passphrase)
        {
            return passphrase == (string)Conf.Instance.Configuration["Secret"];
        }
    }
}
