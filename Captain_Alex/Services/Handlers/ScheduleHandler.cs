using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Captain_Alex.Services.Jobs;

namespace Captain_Alex.Services.Handlers
{
    public class ScheduleHandler
    {
        private readonly StdSchedulerFactory _factory = new StdSchedulerFactory();
        private IScheduler _scheduler = null;
        
        public async Task HandleSchedules()
        {
            // Grab the Scheduler instance from the Factory
            _scheduler = await _factory.GetScheduler();

            // and start it off
            await _scheduler.Start();
    
            await AddPurgeSchedule();
        }

        private Task AddPurgeSchedule()
        {
            // Define the purge job
            IJobDetail job = JobBuilder.Create<PurgeJob>()
                .WithIdentity("purgeJob", "group1")
                .Build();

            // Trigger the job to run daily at 5am GMT+1
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("purgeJobTrigger", "group1")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(5,0))
                .StartNow()
                .Build();

            // Tell quartz to schedule the job using our trigger
            _scheduler.ScheduleJob(job, trigger);
            
            return Task.CompletedTask;
        }
        
    }
}