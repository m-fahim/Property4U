using System;
using System.Web.Http;
using System.Web.Mvc;
using IdentitySample.Models;
using Property4U.Areas.HelpPage.ModelDescriptions;
using Property4U.Areas.HelpPage.Models;

namespace Property4U.Areas.HelpPage.Controllers
{
    
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        private const string ErrorViewName = "Error";

        public HelpController()
            : this(GlobalConfiguration.Configuration)
        {
        }

        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        public HttpConfiguration Configuration { get; private set; }


        public ActionResult Index()
        {
            if (Request.IsAuthenticated && User.IsInRole("Developer"))
            {
                ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
                return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
            }
            else {
                //return RedirectToAction("LogIn", "Account", new { area = "" });
                return RedirectToAction("Index", "ControlDesk", new { area = "" });
            }
        }

        public ActionResult Api(string apiId)
        {
            if (Request.IsAuthenticated && User.IsInRole("Developer"))
            {
                if (!String.IsNullOrEmpty(apiId))
                {
                    HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                    if (apiModel != null)
                    {
                        return View(apiModel);
                    }
                }

                return View(ErrorViewName);
            }
            else
            {
                return RedirectToAction("Index", "ControlDesk", new { area = "" });
            }
        }

        public ActionResult ResourceModel(string modelName)
        {
            if (Request.IsAuthenticated && User.IsInRole("Developer"))
            {
                if (!String.IsNullOrEmpty(modelName))
                {
                    ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                    ModelDescription modelDescription;
                    if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
                    {
                        return View(modelDescription);
                    }
                }

                return View(ErrorViewName);
            }
            else
            {
                return RedirectToAction("Index", "ControlDesk", new { area = "" });
            }
        }
    }
}