using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    using IdentitySample.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Property4U.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using Microsoft.AspNet.Identity;
    using System.Data.Entity;
    using PagedList;
    using System.IO;
    using System.Text;
    using System.Net.Http.Headers;

    [RequireHttps]
    [HandleError]
    public class ControlDeskController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        [Authorize]
        public async Task<ActionResult> Index()
        {
            if (User.IsInRole("Admin")) {
                return RedirectToAction("ActiveAD", "ControlDesk");
            }else if (User.IsInRole("Agent")) {
                return RedirectToAction("ActiveAG", "ControlDesk");
            }else if (User.IsInRole("Member")) {
                return RedirectToAction("ActiveME", "ControlDesk");
            }else if (User.IsInRole("Developer")) {
                return RedirectToAction("ActiveDE", "ControlDesk");
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActiveAD()
        {
            //Services.FeedService feedService = new Services.FeedService();
            //Stopwatch timer = Stopwatch.StartNew();
            //timer.Start();
            var content = await GetCDContentAD();
            //timer.Stop();
            //feeds.TimeTaken = timer.ElapsedMilliseconds;
            return View(content);
        }

        [Authorize(Roles = "Agent")]
        public async Task<ActionResult> ActiveAG()
        {
            var content = await GetCDContentAG();
            return View(content);
        }

        [Authorize(Roles = "Member")]
        public async Task<ActionResult> ActiveME()
        {
            var content = await GetCDContentME();
            return View(content);
        }

        [Authorize(Roles = "Developer")]
        public async Task<ActionResult> ActiveDE()
        {
            var content = await GetCDContentDE();
            return View(content);
        }

        public async Task<ControlDeskActiveAD> GetCDContentAD()
        {
            var usersHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            usersHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var users = await usersHttp.GetStringAsync(GetSiteRoot() + "/api/Account/RegisterUsers");

            var propertiesHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            propertiesHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var properties = await propertiesHttp.GetStringAsync(GetSiteRoot() + "/api/Properties/GetPopularProperties");

            var renewalsHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            renewalsHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var renewals = await renewalsHttp.GetStringAsync(GetSiteRoot() + "/api/Renewals/GetPendingRenewals");

            var ordersHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            ordersHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var orders = await ordersHttp.GetStringAsync(GetSiteRoot() + "/api/Orders/GetPendingOrders");

            var adsHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            adsHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var ads = await adsHttp.GetStringAsync(GetSiteRoot() + "/api/Ads/GetActiveAds");

            //await Task.WhenAll(emails, myTasks, notes, bookmarks);


            ControlDeskActiveAD con = new ControlDeskActiveAD();
            con.Users = Deserialize<IdentityUserRole>(users);
            con.Properties = Deserialize<Property>(properties);
            var Renewals = Deserialize<ObjectCon>(renewals);
            if(Renewals.Count > 0){
                con.RenewalsCost = Renewals[0].ObjectValue;
                con.RenewalsCount = Renewals[0].ObjectValuesCount;
            }
            var Orders = Deserialize<ObjectCon>(orders);
            if (Orders.Count > 0)
            {
                con.OrdersCount = Orders[0].ObjectValuesCount;
            }
            var Ads = Deserialize<ObjectCon>(ads);
            if (Ads.Count > 0)
            {
                con.AdsCount = Ads[0].ObjectValuesCount;
            }

            // Photos, Order Zip and Ads Data Size - ControlDeskActiveAG
            // 1. COALESCE(SUM(),0) - return 0 instead of null
            // 2. CAST(Size AS float) - convert to float
            //try
            //{
            var uploadedDataSize = db.Database.SqlQuery<double>("SELECT (SELECT COALESCE(SUM(CAST(Size AS float)),0) AS SizeP FROM Photo) + (SELECT COALESCE(SUM(CAST(ZipFileSize AS float)),0) AS SizeO FROM [dbo].[Order] AS o) + (SELECT COALESCE(SUM(CAST(ImageSize AS float)),0) AS SizeA FROM Ad) AS Size").FirstOrDefault<double>();
            con.UploadedDataSize = uploadedDataSize;
            //}
            //catch (InvalidOperationException)
            //{
            //    con.PhotosSize = 0.00;
            //}

            var srInfo = db.Database.SqlQuery<ServerInfoAD>("SELECT SERVERPROPERTY('productversion') as 'ProductVersion', SERVERPROPERTY('productlevel') as 'PatchLevel', SERVERPROPERTY('edition') as 'ProductEdition', SERVERPROPERTY('buildclrversion') as 'CLRVersion', SERVERPROPERTY('collation') as 'DefaultCollation', SERVERPROPERTY('instancename') as 'Instance', SERVERPROPERTY('lcid') as 'LCID', SERVERPROPERTY('servername') as 'ServerName'").FirstOrDefault<ServerInfoAD>();
            con.ServerInfo = srInfo;

            return con;
        }
        public async Task<ControlDeskActiveAG> GetCDContentAG()
        {
            var propertiesHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            propertiesHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var properties = await propertiesHttp.GetStringAsync(GetSiteRoot() + "/api/Properties/GetPopularProperties");

            var propertiesProfitHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            propertiesProfitHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var propertiesProfit = await propertiesProfitHttp.GetStringAsync(GetSiteRoot() + "/api/Properties/GetYearlyProfit");

            //await Task.WhenAll(emails, myTasks, notes, bookmarks);

            ControlDeskActiveAG con = new ControlDeskActiveAG();
            con.Properties = Deserialize<Property>(properties);

            var propertiesPro = Deserialize<ObjectCon>(propertiesProfit);
            if(propertiesPro.Count > 0){
                con.PropertiesYearlyProfit = propertiesPro[0].ObjectValue;
            }

            strCurrentUserId = User.Identity.GetUserId();

            // GetActiveBiddingsAG - ControlDeskActiveAG
            var biddingsA = await db.Biddings.Where(r => r.Property.AgentID.Equals(strCurrentUserId) && r.BiddingStatus.ToString().Equals("Active")).ToListAsync();
            con.Biddings = biddingsA.Count;

            // GetPendingOrdersAG - ControlDeskActiveAG
            var ordersP = await db.Orders.Where(o => o.AgentID.Equals(strCurrentUserId) && o.OrderStatus.ToString().Equals("Pending")).ToListAsync();
            con.PendingOrders = ordersP.Count;

            // GetPendingOrdersAG - ControlDeskActiveAG
            //var ordersR = await db.Orders.Where(o => o.AgentID.Equals(strCurrentUserId) && o.OrderStatus.ToString().Equals("Rejected")).ToListAsync();
            //con.PendingOrders = ordersR.Count;

            // GetActiveAdsAG - ControlDeskActiveAG
            var adsR = await db.Ads.Where(a => a.Order.AgentID.Equals(strCurrentUserId) && a.AdStatus.ToString().Equals("Active")).ToListAsync();
            con.ActiveAds = adsR.Count;

            // GetAgreeResponsesAG - ControlDeskActiveAG
            var responsesA = await db.Responses.Where(r => r.AgentID.Equals(strCurrentUserId) && r.ResponseStatus.ToString().Equals("Agree") && r.Request.VisitingDate.Month.Equals(DateTime.Today.Month)).ToListAsync();
            con.ResponsesAgree = responsesA.Count;

            // Photos and Order Zip Data Size - ControlDeskActiveAD
            //try
            //{
            var agentUploadedDataSize = db.Database.SqlQuery<double>("SELECT (SELECT COALESCE(SUM(CAST(Size AS float)),0) AS SizeP FROM Photo as Ph inner join Property as Pr on Ph.propertyID = Pr.ID WHERE Pr.AgentID = '"+ strCurrentUserId + "') + (SELECT COALESCE(SUM(CAST(ZipFileSize AS float)),0) AS SizeO FROM [dbo].[Order] WHERE AgentID = '" + strCurrentUserId + "') AS Size").FirstOrDefault<double>();
            con.AgentUploadedDataSize = agentUploadedDataSize;
            //}
            //catch (InvalidOperationException)
            //{
            //    con.PhotosSize = 0.00;
            //}
            return con;
        }
        public async Task<ControlDeskActiveME> GetCDContentME()
        {
            var avaliableAgentsHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            avaliableAgentsHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var avaliableAgents = await avaliableAgentsHttp.GetStringAsync(GetSiteRoot() + "/api/Account/Agents");

            var propertiesHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            propertiesHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var properties = await propertiesHttp.GetStringAsync(GetSiteRoot() + "/api/Properties/GetPopularProperties");

            var avaliablePropertiesHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            avaliablePropertiesHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var avaliableProperties = await avaliablePropertiesHttp.GetStringAsync(GetSiteRoot() + "/api/Properties/GetAvaliableProperties");

            var biddingsAHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            biddingsAHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var biddingsA = await biddingsAHttp.GetStringAsync(GetSiteRoot() + "/api/Biddings/GetActiveBiddings");

            //await Task.WhenAll(emails, myTasks, notes, bookmarks);

            ControlDeskActiveME con = new ControlDeskActiveME();
            var AvaliableAgentsC = Deserialize<ObjectCon>(avaliableAgents);
            if (AvaliableAgentsC.Count > 0)
            {
                con.AvaliableAgents = AvaliableAgentsC[0].ObjectValuesCount;
            }
            con.Properties = Deserialize<Property>(properties);
            var AvaliablePropertiesC = Deserialize<ObjectCon>(avaliableProperties);
            if (AvaliablePropertiesC.Count > 0) {
                con.AvaliablePropertiesCount = AvaliablePropertiesC[0].ObjectValuesCount;
            }
            var ActiveBiddingsC = Deserialize<ObjectCon>(biddingsA);
            if (ActiveBiddingsC.Count > 0)
            {
                con.ActiveBiddingsCount = ActiveBiddingsC[0].ObjectValuesCount;
            }

            strCurrentUserId = User.Identity.GetUserId();

            // GetAcceptedRequestsME - ControlDeskActiveME
            var requestsA = await db.Requests.Where(r => r.MemberID.Equals(strCurrentUserId) && r.RequestStatus.ToString().Equals("Accepted") && r.VisitingDate.Month.Equals(DateTime.Today.Month)).ToListAsync();
            con.RequestsAccepted = requestsA.Count;

            // GetProcessBidsME - ControlDeskActiveME
            var bidsP = await db.Bids.Where(r => r.MemberID.Equals(strCurrentUserId) && r.BidStatus.ToString().Equals("Process")).ToListAsync();
            con.BidsProcess = bidsP.Count;

            // GetWinnerBidsME - ControlDeskActiveME
            var bidsW = await db.Bids.Where(r => r.MemberID.Equals(strCurrentUserId) && r.BidStatus.ToString().Equals("Winner") && r.BidOn.Year.Equals(DateTime.Today.Year)).ToListAsync();
            con.BidsWin = bidsW.Count;

            return con;
        }
        public async Task<ControlDeskActiveDEPropertyViewModels> GetCDContentDE()
        {
            // Two Models in Single View
            ControlDeskActiveDEPropertyViewModels viewModel = new ControlDeskActiveDEPropertyViewModels();

            var developersConHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            developersConHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var developersCon = await developersConHttp.GetStringAsync(GetSiteRoot() + "/api/Account/Developers");

            var randomDevelopersHttp = new HttpClient();
            // Setting Authorization Header of HttpClient with Token before GetStringAsync
            randomDevelopersHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["accessToken"].Value);
            var randomDevelopers = await randomDevelopersHttp.GetStringAsync(GetSiteRoot() + "/api/Account/RandomDevelopers");

            //await Task.WhenAll(emails, myTasks, notes, bookmarks);

            ControlDeskActiveDE con = new ControlDeskActiveDE();
            var developersC = Deserialize<ObjectCon>(developersCon);
            if (developersC.Count > 0)
            {
                con.DevelopersCount = developersC[0].ObjectValuesCount;
            }
            con.Developers = Deserialize<ApplicationUser>(randomDevelopers);

            // GetSystemTables - ControlDeskActiveDE
            var tables = db.Database.SqlQuery<int>("SELECT COUNT(*) from information_schema.tables WHERE table_type = 'base table'").FirstOrDefault<int>();
            con.APIsDoc = tables - 5;

            // Use DB_sp_spaceused Class to store SqlQuery results from sp_spaceused
            var dbSize = db.Database.SqlQuery<DB_sp_spaceused>("sp_spaceused").FirstOrDefault<DB_sp_spaceused>();
            con.DBSize = dbSize.database_size;

            // Initializing viewModel
            viewModel.ControlDeskActiveDE = con;
            viewModel.Properties = db.Properties.Include(p => p.Address).Include(p => p.Agent).ToList();

            return (viewModel);
        }

        // Get Site Root URL
        public static string GetSiteRoot()
        {
            string port = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            if (port == null || port == "80" || port == "443")
                port = "";
            else
                port = ":" + port;

            string protocol = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (protocol == null || protocol == "0")
                protocol = "http://";
            else
                protocol = "https://";

            string sOut = protocol + System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + port + System.Web.HttpContext.Current.Request.ApplicationPath;

            if (sOut.EndsWith("/"))
            {
                sOut = sOut.Substring(0, sOut.Length - 1);
            }

            return sOut;
        }

        public List<T> Deserialize<T>(string data)
        {
            List<T> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(data);
            return list;
        }

        public async Task<ActionResult> PU_Samples()
        {
            string LoadedData = null;
            try
            {
                using (StreamReader sr = new StreamReader(Path.Combine(Server.MapPath("~/Content/System/Sample-Data.bak")), Encoding.UTF8))
                {
                    LoadedData = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            if (LoadedData != null) {
                //ClearDatabase<ApplicationDbContext>();
                using (System.Data.Entity.DbContextTransaction dbTran = db.Database.BeginTransaction())
                {
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[AspNetUserRoles]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[AspNetRoles]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Ad]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Bid]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Bidding]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Feature]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Feedback]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Order]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Photo]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Renewal]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Response]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Request]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[OfSubType]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Reply]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Review]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Property]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[OfType]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Address]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[Configuration]");
                    await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM [dbo].[AspNetUsers]");
                    await db.Database.ExecuteSqlCommandAsync(@"" + LoadedData );

                    //saves all above operations within one transaction
                    await db.SaveChangesAsync();

                    //commit transaction
                    dbTran.Commit();
                }

            }

            @ViewBag.FileData = LoadedData;
            return View();
        }

        //public static void ClearDatabase<T>() where T : ApplicationDbContext, new()
        //{
        //    using (var context = new T())
        //    {
        //        var tableNames = context.Database.SqlQuery<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME NOT LIKE '%Migration%'").ToList();
        //        foreach (var tableName in tableNames)
        //        {
        //            context.Database.ExecuteSqlCommand(string.Format("DELETE FROM {0}", tableName));
        //        }

        //        context.SaveChanges();
        //    }
        //}
        
    }
}
