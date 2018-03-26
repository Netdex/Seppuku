using Quartz;

namespace Seppuku.Switch
{
    internal class DeadmanJob : IJob
    {
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            L.Trace("Deadman job triggered");
            SwitchControl.Trigger();
        }
    }
}