using System.Web.Mvc;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using JabbR.Services;
using Orchard.Logging;
using System;

namespace JabbR.Infrastructure.Controllers
{
    [ValidateInput(false), Admin]
    [OrchardFeature("JabbR")]
    public class ChatAdminController : Controller
    {
        private readonly ISettingsManager _settingsService;

        public ChatAdminController(ISettingsManager settingsService, IOrchardServices orchardServices)
        {
            _settingsService = settingsService;
            Services = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public Orchard.Logging.ILogger Logger { get; set; }

        public ActionResult Settings()
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Cannot manage Chat")))
                return new HttpUnauthorizedResult();

            var siteKey = _settingsService.Load();

            return View(siteKey);
        }
        [HttpPost]
        public ActionResult Settings(ApplicationSettings viewModel)
        {

            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Cannot manage JabbR")))
                return new HttpUnauthorizedResult();

            try
            {
                _settingsService.Save(viewModel);
                Services.Notifier.Information(T("JabbR settings successfully saved"));

            }
            catch(Exception exc)
            {
                Logger.Error(exc, "Cannot save JabbRSettings");
            }

            return View(viewModel);
        }
    }
}