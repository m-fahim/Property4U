using IdentitySample.Models;
using Property4U.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Diagnostics;

namespace Property4U.Core
{
    public class BiddingJob : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async void Execute(IJobExecutionContext context)
        {

            // Load Updated Task Information to TaskInfo.Info
            SysTask task = new SysTask { Name = context.JobDetail.Key.Name, NextExecutionTime = getNextFireTimeForJob(context.Scheduler, "Core", "Tasks"), StartTime = getStartTimeForJob(context.Scheduler, "Core", "Tasks") };

            // Gain access to an HttpContext object in a Quartz.NET job - TaskInfo.Info
            HttpContext contextH = context.JobDetail.JobDataMap["context"] as HttpContext;
            contextH.Application["Info"] = task;

            using (DbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    // Load System Configurations
                    //var systemConfigurations = ConfigSys.GetSysInfo();
                    var systemConfigurations = await db.Configurations.FindAsync(1);

                    /* Check is there any Bids Results Pending on Bindings with Active Status */
                    var todayBindingResultsAwaited = db.Biddings.Where(b => b.EndDate.Equals(DateTime.Today.Date) && b.BiddingStatus.ToString().Equals("Active"));

                    var todayBindingResultsAwaitedL = await todayBindingResultsAwaited.ToListAsync();
                    if (todayBindingResultsAwaitedL != null && todayBindingResultsAwaitedL.Count != 0)
                    {
                        foreach (var todayBindings in todayBindingResultsAwaitedL)
                        {
                            var winningBid = db.Bids.SqlQuery("SELECT * FROM Bid where BidOn = (select Max(BidOn) FROM Bid WHERE Price = (select MAX(bi.Price) as Price FROM Bid bi INNER JOIN Bidding bg ON bi.BiddingID = bg.ID WHERE price >= MinExp and bi.BiddingID = @p0))", todayBindings.ID);
                            var winningBidL = await winningBid.ToListAsync();
                            if (winningBidL != null && winningBidL.Count != 0)
                            {
                                foreach (var winningB in winningBidL)
                                {
                                    await db.Database.ExecuteSqlCommandAsync("UPDATE Bid SET BidStatus = 0 WHERE ID = {0} AND BiddingID = {1}", winningB.ID, todayBindings.ID);
                                    await db.Database.ExecuteSqlCommandAsync("UPDATE Bid SET BidStatus = 3 WHERE ID != {0} AND BiddingID = {1}", winningB.ID, todayBindings.ID);
                                    await db.Database.ExecuteSqlCommandAsync("UPDATE Bidding SET WinningBid = {0}, BiddingStatus = 2 WHERE ID = {1}", winningB.ID, todayBindings.ID);
                                }
                            }
                            else
                            {
                                await db.Database.ExecuteSqlCommandAsync("UPDATE Bidding SET BiddingStatus = 2 WHERE ID = {0}", todayBindings.ID);
                            }
                        }
                    }

                    /* Check is there any Bindings Pending for Activation Status */
                    var todayBindingActivationAwaited = db.Biddings.Where(b => b.StartDate.Equals(DateTime.Today.Date) && b.BiddingStatus.ToString().Equals("UpComing"));
                    var todayBindingActivationAwaitedL = await todayBindingActivationAwaited.ToListAsync();
                    if (todayBindingActivationAwaitedL != null && todayBindingActivationAwaitedL.Count != 0)
                    {
                        foreach (var todayActiveBindings in todayBindingActivationAwaitedL)
                        {
                            await db.Database.ExecuteSqlCommandAsync("UPDATE Bidding SET BiddingStatus = 1 WHERE ID = {0} AND BiddingStatus = 0", todayActiveBindings.ID);
                        }
                    }

                    /* Update Property Status after expiration and Add Renewal accordingly */
                    var configPropertyRenewal = (int)systemConfigurations.PropertyRenewal;
                    DateTime minusConfigPropertyRenewal = DateTime.Now.Date.AddMonths(-configPropertyRenewal);

                    var todayPropertyRenewalAwaited = db.Properties.Where(p => p.PublishOn.Equals(minusConfigPropertyRenewal) && p.Availability.ToString().Equals("Yes"));

                    var todayPropertyRenewalAwaitedL = await todayPropertyRenewalAwaited.ToListAsync();
                    if (todayPropertyRenewalAwaitedL != null && todayPropertyRenewalAwaitedL.Count != 0)
                    {
                        foreach (var todayPropertyRenewalBlock in todayPropertyRenewalAwaitedL)
                        {
                            await db.Database.ExecuteSqlCommandAsync("UPDATE Property SET Avaliability = 1 WHERE ID = @p0", todayPropertyRenewalBlock.ID);
                            //await db.Database.ExecuteSqlCommandAsync("INSERT INTO Renewal(PropertyID, Description, Price, Status, Dated) values (" + todayPropertyRenewalBlock.ID + ", 'Renewal Alert - " + todayPropertyRenewalBlock.Title + " - Published On (" + todayPropertyRenewalBlock.PublishOn + ")', " + systemConfigurations.RenewalCost + ", 0, " + DateTime.Now.Date + ")");

                            db.Renewals.Add(new Renewal { PropertyID = todayPropertyRenewalBlock.ID, Description = "Renewal Alert - " + todayPropertyRenewalBlock.Title + " - Published On (" + todayPropertyRenewalBlock.PublishOn + ")", Price = systemConfigurations.RenewalCost, Status = ReStatus.Active, Dated = DateTime.Now.Date });
                        }
                    }

                    /* Update Ads Status to expired after AdDuration reached */
                    var todayAdExpirationAwaited = db.Ads.SqlQuery("Select * FROM Ad WHERE DATEADD(day,AdDuration,PostedOn) = DATEDIFF(dd, 0, GETDATE())" /*&& a.AdStatus.ToString().Equals("Active")*/);
                    var todayAdExpirationAwaitedL = await todayAdExpirationAwaited.ToListAsync();
                    if (todayAdExpirationAwaitedL != null && todayAdExpirationAwaitedL.Count != 0)
                    {
                        foreach (var todayExpiredAds in todayAdExpirationAwaitedL)
                        {
                            await db.Database.ExecuteSqlCommandAsync("UPDATE Ad SET AdStatus = 2, Remedies = 'Expiry Alert - " + todayExpiredAds.Title + " - Place new Order for Advertisement" + "'  WHERE ID = @p0", todayExpiredAds.ID);
                            await db.Database.ExecuteSqlCommandAsync("UPDATE dbo.[Order] SET OrderStatus = 4, Remedies = 'Expiry Alert - " + todayExpiredAds.Title + " - Place new Order for Advertisement" + "' WHERE ID = @p0", todayExpiredAds.OrderID);
                        }
                    }

                    await db.SaveChangesAsync();
                    dbTran.Commit();
                }
                catch (Exception)
                {
                    //Rollback transaction if exception occurs
                    dbTran.Rollback();
                }
            }
        }

        private DateTime getNextFireTimeForJob(IScheduler scheduler, string jobName, string groupName = "")
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

        private DateTime getStartTimeForJob(IScheduler scheduler, string jobName, string groupName = "")
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