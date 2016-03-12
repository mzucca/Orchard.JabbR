using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using System.Web.Mvc;

namespace JabbR.Controllers
{
    public class BaseAdminController : Controller, IUpdateModel
    {
        protected readonly ISiteService _siteService;

        public BaseAdminController(
            IOrchardServices services,
            IShapeFactory shapeFactory,
            ISiteService siteService
            )
        {
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;

            _siteService = siteService;
            Services = services;
            Shape = shapeFactory;

        }
        protected dynamic Shape { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }


        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel<TModel>(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }

}
