﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Seppuku.Config;
using Seppuku.Module;
using Seppuku.Module.Utility;

namespace Seppuku.Switch
{
    /// <summary>
    /// Defines most helper methods to manage the deadman's switch
    /// </summary>
    class SwitchControl
    {
        public static void Reset()
        {
            Conf.Set("FailureDate", DateTime.Now.AddSeconds(Conf.Get("GraceTime", double.MaxValue)));
            Conf.Set("Triggered", false);
            Sched.UnscheduleTrigger();
            Sched.ScheduleTrigger(Conf.Get("FailureDate", DateTime.MaxValue));
        }

        public static void Trigger()
        {
            if (!Expired())
            {
                Conf.Set("FailureDate", DateTime.Now.AddMilliseconds(-1));
            }
            Conf.Set("Triggered", true);
            Sched.UnscheduleTrigger();
            ModuleManager.Instance.EmitTrigger();
        }

        public static TimeSpan TimeLeft()
        {
            return Conf.Get("FailureDate", DateTime.Now) - DateTime.Now;
        }

        public static bool Expired()
        {
            return TimeLeft() < TimeSpan.Zero;
        }

        public static bool Triggered()
        {
            return Conf.Get("Triggered", false);
        }

        public static bool Authorized(string secret)
        {
            return secret == SeppukuAuth.GetCurrentToken(Conf.Get<string>("Secret", null));
        }
    }
}
