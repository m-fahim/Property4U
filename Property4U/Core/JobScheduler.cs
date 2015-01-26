using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Core
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<BiddingJob>()
                .WithIdentity("Core", "Tasks")
                .Build();

            // Provide access to an HttpContext object in a Quartz.NET job
            job.JobDataMap["context"] = HttpContext.Current;

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("CoreTrigger", "Tasks")
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);

            // On Start Load Task Information to TaskInfo.Info
            SysTask task = new SysTask { Name = job.Key.Name, NextExecutionTime = getNextFireTimeForJob(scheduler, "Core", "Tasks"), StartTime = getStartTimeForJob(scheduler, "Core", "Tasks") };
            TaskInfo.Info = task;
        }

        private static DateTime getNextFireTimeForJob(IScheduler scheduler, string jobName, string groupName = "")
        {
            JobKey jobKey = new JobKey(jobName, groupName);
            DateTime nextFireTime = DateTime.MinValue;

            bool isJobExisting = scheduler.CheckExists(jobKey);
            if (isJobExisting)
            {
                var detail = scheduler.GetJobDetail(jobKey);
                var triggers = scheduler.GetTriggersOfJob(jobKey);

                if (triggers.Count > 0)
                {
                    var nextFireTimeUtc = triggers[0].GetNextFireTimeUtc();
                    nextFireTime = TimeZone.CurrentTimeZone.ToLocalTime(nextFireTimeUtc.Value.DateTime);
                }
            }

            return (nextFireTime);
        }

        private static DateTime getStartTimeForJob(IScheduler scheduler, string jobName, string groupName = "")
        {
            JobKey jobKey = new JobKey(jobName, groupName);
            DateTime starTime = DateTime.MinValue;

            bool isJobExisting = scheduler.CheckExists(jobKey);
            if (isJobExisting)
            {
                var detail = scheduler.GetJobDetail(jobKey);
                var triggers = scheduler.GetTriggersOfJob(jobKey);

                if (triggers.Count > 0)
                {
                    var startTimeUtc = triggers[0].StartTimeUtc.DateTime;
                    starTime = TimeZone.CurrentTimeZone.ToLocalTime(startTimeUtc);
                }
            }

            return (starTime);
        }
    }
}