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

    [RequireHttps]
    [HandleError]
    public class FrontEndController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        public async Task<ActionResult/*HomeViewModels*/> Index()
        {
            // Two Models in Single View
            HomeViewModels viewModel = new HomeViewModels();
            // Find all types with number of items
            //var properties = db.Properties.Where(p => p.Availability.ToString().Contains("Yes"))
            //      .GroupBy(p => p.OfType)
            //      .Select(p => new ofTypeViewModel
            //      {
            //          Type = p.Key.ToString(),
            //          Count = p.Count()
            //      }).ToList();

            // Find 3 Latest Feature Properties
            var allFeatured = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.Featured.ToString().Equals("Yes")).OrderBy(p => p.ID).Take(3).ToListAsync();

            List<HomePropertyPhotoViewModel> allFeaturedPhoto = new List<HomePropertyPhotoViewModel>();
            foreach (var property in allFeatured)
            {
                HomePropertyPhotoViewModel model = new HomePropertyPhotoViewModel();
                model.Property = property;
                model.Photo = await db.Photos.Where(ph => ph.PropertyID == property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                allFeaturedPhoto.Add(model);
            }
            viewModel.FeaturePropertyPhoto = allFeaturedPhoto.ToList();

            // Find all Types with related SubTypes
            //var types = db.OfSubTypes.Include(o => o.OfType).OrderBy(o => o.OfTypeID).ToList();
            //viewModel.Types = types;
            var allTypeSubTypes = await db.OfTypes.ToListAsync();

            List<HomeTypeSubTypesViewModel> allTypeSubTypesL = new List<HomeTypeSubTypesViewModel>();
            foreach (var type in allTypeSubTypes)
            { 
                HomeTypeSubTypesViewModel model = new HomeTypeSubTypesViewModel();
                model.Type = type;
                model.SubTypes = await db.OfSubTypes.Where(o => o.OfTypeID == type.ID).OrderBy(o => o.ID).ToListAsync();
                //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                allTypeSubTypesL.Add(model);
            }
            viewModel.TypeSubTypes = allTypeSubTypesL.ToList();

            // Find all For with number of items count
            var fors = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes"))
                 .GroupBy(p => p.For)
                 .Select(p => new HomeForViewModel
                 {
                     Title = p.Key.ToString(),
                     Count = p.Count()
                 }).ToListAsync();
            viewModel.For = fors;

            // Find all AllowBidding with number of items count
            var biddings = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes"))
                 .GroupBy(p => p.AllowBidding)
                 .Select(p => new HomeBiddingsViewModel
                 {
                     Status = p.Key.ToString(),
                     Count = p.Count()
                 }).ToListAsync();
            viewModel.Biddings = biddings;

            var allProperties = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes")).OrderByDescending(p => p.Views).Take(6).ToListAsync();

            List<HomePropertyPhotoViewModel> allPropertyPhoto = new List<HomePropertyPhotoViewModel>();
            foreach (var property in allProperties)
            {
                HomePropertyPhotoViewModel model = new HomePropertyPhotoViewModel();
                model.Property = property;
                model.Photo = await db.Photos.Where(ph => ph.PropertyID == property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                allPropertyPhoto.Add(model);
            }
            viewModel.PropertyPhoto = allPropertyPhoto.ToList();

            var allRecents = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.AllowBidding.ToString().Contains("Disabled")).OrderBy(p => p.ID).Take(4).ToListAsync();

            List<HomeRecentPhotoViewModel> allRecentPhoto = new List<HomeRecentPhotoViewModel>();
            foreach (var property in allRecents)
            {
                HomeRecentPhotoViewModel model = new HomeRecentPhotoViewModel();
                model.Property = property;
                model.Photo = await db.Photos.Where(ph => ph.PropertyID == property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                allRecentPhoto.Add(model);
            }
            viewModel.RecentPropertyPhoto = allRecentPhoto.ToList();

            var allBiddings = await db.Biddings.Where(b => b.BiddingStatus.ToString().Contains("Active")).OrderByDescending(b => b.ID).Take(4).ToListAsync();

            List<BiddingsPhotoViewModel> allBiddingPhoto = new List<BiddingsPhotoViewModel>();
            foreach (var bidding in allBiddings)
            {
                BiddingsPhotoViewModel model = new BiddingsPhotoViewModel();
                model.Bidding = bidding;
                model.Property = bidding.Property;
                model.OfSubType = (model.Property.OfSubType != null) ? await db.OfSubTypes.FindAsync(model.Property.OfSubType) : null;
                model.Photo = await db.Photos.Where(ph => ph.PropertyID == model.Property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                allBiddingPhoto.Add(model);
            }
            viewModel.BiddingPropertyPhoto = allBiddingPhoto.ToList();
                
            return View(viewModel);
        }

        public async Task<ActionResult/*HomeViewModels*/> Properties(string filterType, string filterSuType, string filterFor, string filterBidding, int? filterRangeMi, int? filterRangeMx, int? page)
        {
            // Two Models in Single View
            HomeViewModels viewModel = new HomeViewModels();
           
            // Find all Types with related SubTypes
            //var types = db.OfSubTypes.Include(o => o.OfType).OrderBy(o => o.OfTypeID).ToList();
            //viewModel.Types = types;
            var allTypeSubTypes = await db.OfTypes.ToListAsync();

            List<HomeTypeSubTypesViewModel> allTypeSubTypesL = new List<HomeTypeSubTypesViewModel>();
            foreach (var type in allTypeSubTypes)
            {
                HomeTypeSubTypesViewModel model = new HomeTypeSubTypesViewModel();
                model.Type = type;
                model.SubTypes = await db.OfSubTypes.Where(o => o.OfTypeID == type.ID).OrderBy(o => o.ID).ToListAsync();
                //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                allTypeSubTypesL.Add(model);
            }
            viewModel.TypeSubTypes = allTypeSubTypesL.ToList();

            IEnumerable<Property> allProperties = null;
            //ViewBag.CurrentSort = filterType;
            //ViewBag.TypeSortParm = String.IsNullOrEmpty(filterType) ? filterType : "";
            if (filterType != null && filterSuType != null && page == null)
            {
                page = 1;
                var subType = await db.OfSubTypes.Where(sb => sb.Title.ToString().Equals(filterSuType)).FirstOrDefaultAsync();

                allProperties = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.OfType.Title.ToString().Equals(filterType) && p.OfSubType == subType.ID).OrderBy(p => p.Views).ToListAsync();
            }
            else if (filterType != null  && page == null)
            {
                page = 1;

                allProperties = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.OfType.Title.ToString().Equals(filterType)).OrderBy(p => p.Views).ToListAsync();
            }
            else
            {
                allProperties = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes")).OrderBy(p => p.Views).ToListAsync();
            }

            if (filterFor != null)
            {
                allProperties = allProperties.Where(p => p.For.ToString().Contains(filterFor)).ToList();
            }

            if (filterBidding != null)
            {
                allProperties = allProperties.Where(p => p.AllowBidding.ToString().Contains(filterBidding)).ToList();
            }

            if (filterRangeMi != null && filterRangeMx != null)
            {
                allProperties = allProperties.Where(p => p.Price > filterRangeMi && p.Price < filterRangeMx).ToList();
            }

            ViewBag.CurrentType = filterType;
            ViewBag.CurrentSubType = filterSuType;
            ViewBag.CurrentFor = filterFor;
            ViewBag.CurrentBidding = filterBidding;
            ViewBag.CurrentRangeMi = filterRangeMi;
            ViewBag.CurrentRangeMx = filterRangeMx;
            

            // Update Side menu items count according to filter
            if ((filterType != null && filterSuType != null))
            {
                var subType = await db.OfSubTypes.Where(sb => sb.Title.ToString().Equals(filterSuType)).FirstOrDefaultAsync();
                // Find all For with number of items count
                var fors = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.OfType.Title.ToString().Equals(filterType) && p.OfSubType == subType.ID)
                     .GroupBy(p => p.For)
                     .Select(p => new HomeForViewModel
                     {
                         Title = p.Key.ToString(),
                         Count = p.Count()
                     }).ToListAsync();
                viewModel.For = fors;

                // Find all AllowBidding with number of items count
                var biddings = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.OfType.Title.ToString().Equals(filterType) && p.OfSubType == subType.ID)
                     .GroupBy(p => p.AllowBidding)
                     .Select(p => new HomeBiddingsViewModel
                     {
                         Status = p.Key.ToString(),
                         Count = p.Count()
                     }).ToListAsync();
                viewModel.Biddings = biddings;
            }
            else if (filterType != null && filterSuType == null)
            {
                // Find all For with number of items count
                var fors = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.OfType.Title.ToString().Equals(filterType))
                     .GroupBy(p => p.For)
                     .Select(p => new HomeForViewModel
                     {
                         Title = p.Key.ToString(),
                         Count = p.Count()
                     }).ToListAsync();
                viewModel.For = fors;

                // Find all AllowBidding with number of items count
                var biddings = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.OfType.Title.ToString().Equals(filterType))
                     .GroupBy(p => p.AllowBidding)
                     .Select(p => new HomeBiddingsViewModel
                     {
                         Status = p.Key.ToString(),
                         Count = p.Count()
                     }).ToListAsync();
                viewModel.Biddings = biddings;
            }
            else if (filterType == null && filterSuType == null && filterFor == null && filterBidding == null && filterRangeMi == null && filterRangeMx == null)
            {
                // Find all For with number of items count
                var fors = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes"))
                     .GroupBy(p => p.For)
                     .Select(p => new HomeForViewModel
                     {
                         Title = p.Key.ToString(),
                         Count = p.Count()
                     }).ToListAsync();
                viewModel.For = fors;

                // Find all AllowBidding with number of items count
                var biddings = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes"))
                     .GroupBy(p => p.AllowBidding)
                     .Select(p => new HomeBiddingsViewModel
                     {
                         Status = p.Key.ToString(),
                         Count = p.Count()
                     }).ToListAsync();
                viewModel.Biddings = biddings;
            }

            if (filterFor != null || filterBidding != null || (filterRangeMi != null && filterRangeMx != null))
            {

                if (viewModel.For == null)
                {
                    // Find all For with number of items count
                    var fors = allProperties.Where(p => p.Availability.ToString().Contains("Yes"))
                            .GroupBy(p => p.For)
                            .Select(p => new HomeForViewModel
                            {
                                Title = p.Key.ToString(),
                                Count = p.Count()
                            }).ToList();
                    viewModel.For = fors;
                }

                if (viewModel.Biddings == null)
                {
                    // Find all AllowBidding with number of items count
                    var biddings = allProperties.Where(p => p.Availability.ToString().Contains("Yes"))
                            .GroupBy(p => p.AllowBidding)
                            .Select(p => new HomeBiddingsViewModel
                            {
                                Status = p.Key.ToString(),
                                Count = p.Count()
                            }).ToList();
                    viewModel.Biddings = biddings;
                }
            }


            List<HomePropertyPhotoViewModel> allPropertyPhoto = new List<HomePropertyPhotoViewModel>();
            foreach (var property in allProperties)
            {
                HomePropertyPhotoViewModel model = new HomePropertyPhotoViewModel();
                model.Property = property;
                model.Photo = await db.Photos.Where(ph => ph.PropertyID == property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                allPropertyPhoto.Add(model);
            }

            int pageSize = 9;
            int pageNumber = (page ?? 1);
            viewModel.PropertyPhotoFilterable = allPropertyPhoto.ToPagedList(pageNumber, pageSize);

            //if (!String.IsNullOrEmpty(filterType))
            //{
            //    properties = properties.Where(p => p.Title.ToUpper().Contains(searchProperty.ToUpper())
            //                           || p.Title.ToUpper().Contains(searchProperty.ToUpper()));
            //}

            
            //return View(properties.ToPagedList(pageNumber, pageSize));

            return View(viewModel);
        }

        public async Task<ActionResult/*HomeViewModels*/> Details(int PID, int? page)
        {
            if (PID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetailsPropertyPhotoViewModel viewModel = new DetailsPropertyPhotoViewModel();
            viewModel.Property = await db.Properties.Where(p => p.ID == PID && p.Availability.ToString().Equals("Yes")).FirstOrDefaultAsync();


            if (viewModel.Property != null)
            {
                // Update Property Views On Each User Visit
                Property propertyUpdate = await db.Properties.FindAsync(PID);
                propertyUpdate.Views = (propertyUpdate.Views.ToString().Equals("")) ? 1 : propertyUpdate.Views + 1;
                await db.SaveChangesAsync();

                var subType = await db.OfSubTypes.FindAsync(viewModel.Property.OfSubType);
                viewModel.SubType = subType.Title;

                ViewBag.CurrentPID = PID;
                // Reviews Pagination
                if (page == null)
                {
                    page = 1;
                }

                var pro = db.Photos.Where(ph => ph.PropertyID == PID);
                viewModel.Photos = (pro.Count() > 0) ? await pro.ToListAsync() : null;

                // Find all Properties with number of items count
                var properties = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.AgentID == viewModel.Property.AgentID)
                     .GroupBy(p => p.AgentID)
                     .Select(p => new DetailsAgentProfile
                     {
                         Count = p.Count()
                     }).FirstOrDefaultAsync();
                viewModel.AgentProperties = properties;

                // Find all Biddings with number of items count
                var biddings = await db.Biddings.Where(p => p.BiddingStatus.ToString().Contains("Active") && p.Property.AgentID == viewModel.Property.AgentID)
                     .GroupBy(p => p.Property.AgentID)
                     .Select(p => new DetailsAgentProfile
                     {
                         Count = p.Count()
                     }).FirstOrDefaultAsync();
                viewModel.AgentBiddings = biddings;

                var allProperties = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.OfTypeID == viewModel.Property.OfTypeID && p.ID != viewModel.Property.ID).OrderBy(p => p.Views).Take(6).ToListAsync();

                List<HomePropertyPhotoViewModel> allPropertyPhoto = new List<HomePropertyPhotoViewModel>();
                foreach (var property in allProperties)
                {
                    HomePropertyPhotoViewModel model = new HomePropertyPhotoViewModel();
                    model.Property = property;
                    model.Photo = await db.Photos.Where(ph => ph.PropertyID == property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                    //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                    allPropertyPhoto.Add(model);
                }
                viewModel.RelatedProperties = allPropertyPhoto.ToList();

                var allReviews = await db.Reviews.Where(r => r.PropertyID == PID).ToListAsync();

                List<DetailsReviewRepliesViewModel> allReviewReplyL = new List<DetailsReviewRepliesViewModel>();
                foreach (var review in allReviews)
                {
                    DetailsReviewRepliesViewModel model = new DetailsReviewRepliesViewModel();
                    model.Review = review;
                    model.Replies = await db.Replies.Where(re => re.ReviewID == review.ID).OrderBy(re => re.ID).ToListAsync();
                    allReviewReplyL.Add(model);
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                viewModel.Reviews = allReviewReplyL.ToPagedList(pageNumber, pageSize);

                // Find all Reviews with number of items count
                var allReviewsC = await db.Reviews.Where(r => r.PropertyID == PID)
                     .GroupBy(r => r.ID)
                     .Select(p => new DetailsPropertyRatingViewModel
                     {
                         SumRatings = p.Sum(r => r.Rating),
                         Total = p.Count()
                     }).FirstOrDefaultAsync();
                int results = (allReviewsC != null) ? (allReviewsC.SumRatings / allReviewsC.Total) : 0;
                viewModel.PropertyRating = results;
            }
            else {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(viewModel);
        }
        public async Task<ActionResult> Biddings(string sortOrder, string currentFilter, string searchBidding, string status, string propertyView, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentView = propertyView;
            ViewBag.CurrentStatus = status;

            // Biddings Pagination
            if (searchBidding != null)
            {
                page = 1;
            }
            else
            {
                searchBidding = currentFilter;
            }

            ViewBag.CurrentFilter = searchBidding;

            BiddingsPropertyPhotoViewModel viewModel = new BiddingsPropertyPhotoViewModel();
            
            IEnumerable<Bidding> allBiddings = null;
            if (status != null)
            {
                allBiddings = await db.Biddings.Where(b => b.BiddingStatus.ToString().Contains(status)).OrderByDescending(b => b.ID).ToListAsync();
            }else{
                allBiddings = await db.Biddings.OrderByDescending(b => b.ID).ToListAsync();
            }

            if (!String.IsNullOrEmpty(searchBidding))
            {
                allBiddings = allBiddings.Where(b => b.Title.ToUpper().Contains(searchBidding.ToUpper())
                                       || b.Title.ToUpper().Contains(searchBidding.ToUpper()));
            }

            switch (sortOrder)
            {
                //case "type_desc":
                //    properties = properties.OrderByDescending(p => p.OfType);
                //    break;
                case "Type":
                    allBiddings = allBiddings.OrderBy(b => b.Property.For);
                    break;
                case "Price":
                    allBiddings = allBiddings.OrderBy(b => b.Property.Price);
                    break;
                case "Date":
                    allBiddings = allBiddings.OrderByDescending(b => b.PostedOn);
                    break;
                default:
                    allBiddings = allBiddings.OrderByDescending(b => b.ID);
                    break;
            }

            List<BiddingsPhotoViewModel> allBiddingPhoto = new List<BiddingsPhotoViewModel>();
            foreach (var bidding in allBiddings)
            {
                BiddingsPhotoViewModel model = new BiddingsPhotoViewModel();
                model.Bidding = bidding;
                model.Property = bidding.Property;
                model.OfSubType = (model.Property.OfSubType != null) ? await db.OfSubTypes.FindAsync(model.Property.OfSubType) : null;
                model.Photo = await db.Photos.Where(ph => ph.PropertyID == model.Property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                allBiddingPhoto.Add(model);
            }

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            viewModel.BiddingsPhotoPaged = allBiddingPhoto.ToPagedList(pageNumber, pageSize);

            return View(viewModel);
        }

        public async Task<ActionResult/*HomeViewModels*/> PlaceBid(int BID, int? page)
        {
            if (BID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PlaceBidPropertyPhotoViewModel viewModel = new PlaceBidPropertyPhotoViewModel();
            viewModel.Bidding = await db.Biddings.Where(b => b.ID == BID).FirstOrDefaultAsync();


            if (viewModel.Bidding != null)
            {
                var subType = await db.OfSubTypes.FindAsync(viewModel.Bidding.Property.OfSubType);
                viewModel.SubType = subType.Title;

                ViewBag.CurrentBID = BID;
                // Bids Pagination
                if (page == null)
                {
                    page = 1;
                }

                if (viewModel.Bidding.WinningBid != null) {
                    viewModel.WinningBid = await db.Bids.FindAsync(viewModel.Bidding.WinningBid);
                }

                viewModel.Photo = db.Photos.Where(ph => ph.PropertyID == viewModel.Bidding.PropertyID).FirstOrDefault();

                // Find all Properties with number of items count
                var properties = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes") && p.AgentID == viewModel.Bidding.Property.AgentID)
                     .GroupBy(p => p.AgentID)
                     .Select(p => new DetailsAgentProfile
                     {
                         Count = p.Count()
                     }).FirstOrDefaultAsync();
                viewModel.AgentProperties = properties;

                // Find all Biddings with number of items count
                var biddings = await db.Biddings.Where(p => p.BiddingStatus.ToString().Contains("Active") && p.Property.AgentID == viewModel.Bidding.Property.AgentID)
                     .GroupBy(p => p.Property.AgentID)
                     .Select(p => new DetailsAgentProfile
                     {
                         Count = p.Count()
                     }).FirstOrDefaultAsync();
                viewModel.AgentBiddings = biddings;

                var allBiddings = await db.Biddings.Where(p => p.BiddingStatus.ToString().Contains("Active") && p.Property.OfTypeID == viewModel.Bidding.Property.OfTypeID && p.ID != viewModel.Bidding.ID).OrderBy(p => p.Property.Views).Take(4).ToListAsync();

                List<BiddingPhotoViewModel> allBiddingPhoto = new List<BiddingPhotoViewModel>();
                foreach (var bidding in allBiddings)
                {
                    BiddingPhotoViewModel model = new BiddingPhotoViewModel();
                    model.Bidding = bidding;
                    model.Photo = await db.Photos.Where(ph => ph.PropertyID == model.Bidding.PropertyID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                    //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                    allBiddingPhoto.Add(model);
                }
                viewModel.RelatedBiddings = allBiddingPhoto.ToList();

                var allBids = await db.Bids.Where(r => r.BiddingID == BID).ToListAsync();
               
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                viewModel.Bids = allBids.ToPagedList(pageNumber, pageSize);

            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(viewModel);
        }

        public async Task<ActionResult> Search(string filterSearch, string currentFilter, string Sr, string propertyView, int? page)
        {
            ViewBag.CurrentSearch = filterSearch;
            ViewBag.CurrentView = propertyView;

            // Biddings Pagination
            if (Sr != null)
            {
                page = 1;
            }
            else
            {
                Sr = currentFilter;
            }

            ViewBag.CurrentFilter = Sr;

            SearchBiddingsPropertyPhViewModel viewModel = new SearchBiddingsPropertyPhViewModel();

            IEnumerable<Property> allProperties = null;
            IEnumerable<Bidding> allBiddings = null;
            if ((!String.IsNullOrEmpty(Sr) && filterSearch == "Properties") || (!String.IsNullOrEmpty(Sr) && filterSearch == null))
            {
                allProperties = await db.Properties.Where(p => p.Availability.ToString().Contains("Yes")).OrderByDescending(p => p.ID).ToListAsync();
                allProperties = allProperties.Where(p => p.ID.ToString().Contains(Sr.ToUpper()) || p.Title.ToUpper().Contains(Sr.ToUpper()) || p.Agent.FullName.ToUpper().Contains(Sr.ToUpper()) );
                List<HomePropertyPhotoViewModel> allPropertyPhoto = new List<HomePropertyPhotoViewModel>();
                foreach (var property in allProperties)
                {
                    HomePropertyPhotoViewModel model = new HomePropertyPhotoViewModel();
                    model.Property = property;
                    model.Photo = await db.Photos.Where(ph => ph.PropertyID == property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                    //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                    allPropertyPhoto.Add(model);
                }

                int pageSize = 12;
                int pageNumber = (page ?? 1);
                viewModel.PropertyPhotoPaged = allPropertyPhoto.ToPagedList(pageNumber, pageSize);
            }
            else if (Sr != null && filterSearch == "Biddings" )
            {
                allBiddings = await db.Biddings.OrderByDescending(b => b.ID).ToListAsync();
                allBiddings = allBiddings.Where(b => b.ID.ToString().Contains(Sr.ToUpper()) || b.Title.ToUpper().Contains(Sr.ToUpper()) || b.Property.Agent.FullName.ToUpper().Contains(Sr.ToUpper()));

                List<BiddingsPhotoViewModel> allBiddingPhoto = new List<BiddingsPhotoViewModel>();
                foreach (var bidding in allBiddings)
                {
                    BiddingsPhotoViewModel model = new BiddingsPhotoViewModel();
                    model.Bidding = bidding;
                    model.Property = bidding.Property;
                    model.Photo = await db.Photos.Where(ph => ph.PropertyID == model.Property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                    //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                    allBiddingPhoto.Add(model);
                }

                int pageSize = 12;
                int pageNumber = (page ?? 1);
                viewModel.BiddingsPhotoPaged = allBiddingPhoto.ToPagedList(pageNumber, pageSize);
            }
            else
            {
                ViewBag.CurrentSearch = "Properties";
            }

            

            return View(viewModel);
        }

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

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "About Us";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us";

            return View();
        }

        public async Task<ActionResult> Wishlist(int? totalWishlistItems, string WishlistItems, string WishlistSection, int? pageProperty, int? pageBidding)
        {
            ViewBag.Message = "Wishlist";
            ViewBag.CurrentItems = WishlistItems;
            ViewBag.CurrentWishlistSection = WishlistSection;

            List<string> selectedPIDItemsL = new List<string>();
            List<string> selectedBIDItemsL = new List<string>();
            string selectedPIDItems = null;
            string selectedBIDItems = null;

            if (WishlistItems == null || WishlistItems == "")
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "FrontEnd");

            string last = WishlistItems.Split(new[] { ',' }).Last();
            foreach (var item in WishlistItems.Split(new[] { ',' }))
            {
                var type = (item.Substring(0, 2).Equals("11"))? "PID":"BID";
                if (type.Equals("PID"))
                {
                    var PID = item.Substring(2);
                    selectedPIDItemsL.Add(PID);
                }
                else
                {
                    var BID = item.Substring(2);
                    selectedBIDItemsL.Add(BID);
                }
            }

            string lastP = (selectedPIDItemsL.Count != 0) ? selectedPIDItemsL.Last() : null;
            foreach (var itemP in selectedPIDItemsL)
            {
                if (itemP.Equals(lastP))
                {
                    selectedPIDItems += "ID = " + itemP;
                }
                else
                {
                    selectedPIDItems += "ID = " + itemP + " OR ";
                }
            }

            string lastB = (selectedBIDItemsL.Count != 0) ? selectedBIDItemsL.Last() : null;
            foreach (var itemB in selectedBIDItemsL)
            {
                if (itemB.Equals(lastB))
                {
                    selectedBIDItems += "ID = " + itemB;
                }
                else
                {
                    selectedBIDItems += "ID = " + itemB + " OR ";
                }
            }

            SearchBiddingsPropertyPhViewModel viewModel = new SearchBiddingsPropertyPhViewModel();

            IEnumerable<Property> allProperties = null;
            IEnumerable<Bidding> allBiddings = null;
            if (!String.IsNullOrEmpty(selectedPIDItems))
            {
                allProperties = await db.Properties.SqlQuery("SELECT * FROM Property WHERE " + selectedPIDItems).ToListAsync();
                List<HomePropertyPhotoViewModel> allPropertyPhoto = new List<HomePropertyPhotoViewModel>();
                foreach (var property in allProperties)
                {
                    HomePropertyPhotoViewModel model = new HomePropertyPhotoViewModel();
                    model.Property = property;
                    model.Photo = await db.Photos.Where(ph => ph.PropertyID == property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                    //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                    allPropertyPhoto.Add(model);
                }

                int pageSize = 9;
                int pageNumber = (pageProperty ?? 1);
                viewModel.PropertyPhotoPaged = allPropertyPhoto.ToPagedList(pageNumber, pageSize);

            }
            if (!String.IsNullOrEmpty(selectedBIDItems))
            {
                allBiddings = await db.Biddings.SqlQuery("SELECT * FROM Bidding WHERE " + selectedBIDItems).ToListAsync();

                List<BiddingsPhotoViewModel> allBiddingPhoto = new List<BiddingsPhotoViewModel>();
                foreach (var bidding in allBiddings)
                {
                    BiddingsPhotoViewModel model = new BiddingsPhotoViewModel();
                    model.Bidding = bidding;
                    model.Property = bidding.Property;
                    model.Photo = await db.Photos.Where(ph => ph.PropertyID == model.Property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                    //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();
                    allBiddingPhoto.Add(model);
                }

                int pageSize = 9;
                int pageNumber = (pageBidding ?? 1);
                viewModel.BiddingsPhotoPaged = allBiddingPhoto.ToPagedList(pageNumber, pageSize);
            }

            return View(viewModel);
        }

        public async Task<ActionResult> Comparison(int? totalComparelistItems, string ComparelistItems)
        {
            ViewBag.Message = "Property Comparison";

            string selectedPIDItems = null;

            if (ComparelistItems == null || ComparelistItems == "")
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "FrontEnd");

            string last = ComparelistItems.Split(new[] { ',' }).Last();
            foreach (var item in ComparelistItems.Split(new[] { ',' }))
            {
                if (item.Equals(last))
                {
                    selectedPIDItems += "ID = " + item;
                }
                else
                {
                    selectedPIDItems += "ID = " + item + " OR ";
                }
            }

            CompareViewModel viewModel = new CompareViewModel();

            IEnumerable<Property> allProperties = null;

            if (!String.IsNullOrEmpty(selectedPIDItems))
            {
                allProperties = await db.Properties.SqlQuery("SELECT * FROM Property WHERE " + selectedPIDItems).ToListAsync();
                List<ComparePropertyReviewsViewModel> allPropertyPhotoReview = new List<ComparePropertyReviewsViewModel>();
                foreach (var property in allProperties)
                {
                    ComparePropertyReviewsViewModel model = new ComparePropertyReviewsViewModel();
                    model.Property = property;
                    model.Photo = await db.Photos.Where(ph => ph.PropertyID == property.ID).OrderBy(ph => ph.ID).Take(1).SingleOrDefaultAsync();
                    //model.Products = db.Photos.Where(ph => ph.PropertyID == property.ID).ToList();

                    var subType = await db.OfSubTypes.Where(s => s.ID == model.Property.OfSubType).FirstOrDefaultAsync();
                    model.SubType = subType.Title;

                    DetailsAgentProfile ReviewsCount = await db.Reviews.Where(r => r.PropertyID == model.Property.ID)
                         .GroupBy(r => r.Property.AgentID)
                         .Select(r => new DetailsAgentProfile
                         {
                             Count = r.Count()
                         }).FirstOrDefaultAsync();
                    model.ReviewsCount = (ReviewsCount != null) ?  ReviewsCount.Count : 0;

                    // Find all Reviews with number of items count
                    var allReviewsC = await db.Reviews.Where(r => r.PropertyID == model.Property.ID)
                         .GroupBy(r => r.ID)
                         .Select(p => new DetailsPropertyRatingViewModel
                         {
                             SumRatings = p.Sum(r => r.Rating),
                             Total = p.Count()
                         }).FirstOrDefaultAsync();
                    int results = (allReviewsC != null) ? (allReviewsC.SumRatings / allReviewsC.Total) : 0;
                    model.PropertyRating = results;

                    allPropertyPhotoReview.Add(model);
                }
                viewModel.PropertyPhotoReview = allPropertyPhotoReview.ToList();

            }

            return View(viewModel);
        }
    }
}