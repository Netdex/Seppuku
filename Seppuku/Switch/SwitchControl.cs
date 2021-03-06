﻿using System;
using Seppuku.Config;
using Seppuku.Module;

namespace Seppuku.Switch
{
    /// <summary>
    ///     Defines most helper methods to manage the deadman's switch
    /// </summary>
    internal class SwitchControl
    {
        private static NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        public static void Reset()
        {
            L.Trace("Resetting switch");
            Configuration.Set("FailureDate", DateTime.Now.AddSeconds(Configuration.Get<double>("GraceTime")));
            Configuration.Set("Triggered", false);
            Sched.UnscheduleTrigger();
            Sched.ScheduleTrigger(Configuration.Get("FailureDate", DateTime.MaxValue));
            ModuleManager.Instance.Emit(EmitType.Reset);
        }

        public static void Trigger()
        {
            L.Trace("Triggering switch");
            // force expiry if that hasn't happened yet
            if (!IsExpired)
                Configuration.Set("FailureDate", DateTime.Now.AddMilliseconds(-1));
            Configuration.Set("Triggered", true);
            Sched.UnscheduleTrigger();
            ModuleManager.Instance.Emit(EmitType.Trigger);
        }

        public static TimeSpan TimeLeft()
        {
            return Configuration.Get("FailureDate", DateTime.Now) - DateTime.Now;
        }

        public static bool IsExpired => TimeLeft() < TimeSpan.Zero;
        public static bool IsTriggered => Configuration.Get("Triggered", false);
        public static bool IsAuthorized(string secret) =>
            secret == SeppukuModule.GetCurrentToken(Configuration.Get<string>("Secret"));

    }
}