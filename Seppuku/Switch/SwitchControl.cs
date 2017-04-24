using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seppuku.Config;

namespace Seppuku.Switch
{
    class SwitchControl
    {
        public static void Reset()
        {
            Conf.Configuration.FailureDate = DateTime.Now + Conf.Configuration.GraceTime;
            Conf.Save();
            Sched.UnscheduleTrigger();
            Sched.ScheduleTrigger(Conf.Configuration.FailureDate);
        }

        public static TimeSpan TimeLeft()
        {
            return Conf.Configuration.FailureDate - DateTime.Now;
        }

        public static bool Expired()
        {
            return TimeLeft() < TimeSpan.Zero;
        }
    }
}
