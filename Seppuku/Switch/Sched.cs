using System;
using Quartz;
using Quartz.Impl;

namespace Seppuku.Switch
{
    /// <summary>
    /// Global scheduler for deadman's switch triggers
    /// </summary>
    class Sched
    {
        private const string GroupKey = "seppuku";
        private const string JobKey = "deadman_execution_job";
        private const string TriggerKey = "deadman_execution_trigger";

        private static IScheduler Scheduler { get; set; }
        private static IJobDetail TriggerJob { get; set; }

        public static bool Initialize()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            TriggerJob = JobBuilder.Create<DeadmanJob>()
                .WithIdentity(JobKey, GroupKey)
                .Build();
            return true;
        }

        public static void ScheduleTrigger(DateTime date)
        {
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity(TriggerKey, GroupKey)
                .StartAt(date)
                .ForJob(JobKey, GroupKey)
                .Build();
            
            Scheduler.ScheduleJob(TriggerJob, trigger);
        }

        public static void UnscheduleTrigger()
        {
            Scheduler.UnscheduleJob(new TriggerKey(TriggerKey, GroupKey));
        }
    }
}
