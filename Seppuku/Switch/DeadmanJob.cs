using Quartz;

namespace Seppuku.Switch
{
    internal class DeadmanJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SwitchControl.Trigger();
        }
    }
}