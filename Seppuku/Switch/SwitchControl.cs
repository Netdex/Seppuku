using System;
using Seppuku.Config;
using Seppuku.Module;
using Seppuku.Module.Utility;

namespace Seppuku.Switch
{
    /// <summary>
    ///     Defines most helper methods to manage the deadman's switch
    /// </summary>
    internal class SwitchControl
    {
        public static void Reset()
        {
            Conf.Set("FailureDate", DateTime.Now.AddSeconds(Conf.Get<double>("GraceTime", Conf.DefaultConf)));
            Conf.Set("Triggered", false);
            Sched.UnscheduleTrigger();
            Sched.ScheduleTrigger(Conf.Get("FailureDate", DateTime.MaxValue));
            ModuleManager.Emit(EmitType.Reset);
        }

        public static void Trigger()
        {
            if (!IsExpired)
                Conf.Set("FailureDate", DateTime.Now.AddMilliseconds(-1));
            Conf.Set("Triggered", true);
            Sched.UnscheduleTrigger();
            ModuleManager.Emit(EmitType.Trigger);
        }

        public static TimeSpan TimeLeft()
        {
            return Conf.Get("FailureDate", DateTime.Now) - DateTime.Now;
        }

        public static bool IsExpired => TimeLeft() < TimeSpan.Zero;
        public static bool IsTriggered => Conf.Get("Triggered", false);
        public static bool IsAuthorized(string secret) =>
            secret == SeppukuAuth.GetCurrentToken(Conf.Get<string>("Secret", Conf.DefaultConf));

    }
}