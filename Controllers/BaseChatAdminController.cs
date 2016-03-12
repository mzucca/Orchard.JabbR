using JabbR.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Settings;

namespace JabbR.Controllers
{
    public class BaseChatAdminController: BaseAdminController
    {
        protected readonly IJabbrRepository _repository;

        public BaseChatAdminController(
            IOrchardServices services,
            IShapeFactory shapeFactory,
            ISiteService siteService,
            IJabbrRepository repository
            ) : base (services,shapeFactory,siteService)
        {
            _repository = repository;
        }

    }
}