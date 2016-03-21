using JabbR.Infrastructure;
using Orchard;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.UI.Resources;

namespace JabbR.Elements
{
    public class ChatDriver : ElementDriver<ChatElement>
    {
        private readonly IResourceManager _resourceManager;
        private readonly IOrchardServices _services;

        public ChatDriver(IOrchardServices services, IResourceManager resourceManager )
        {
            _resourceManager = resourceManager;
            _services = services;
        }
        protected override EditorResult OnBuildEditor(ChatElement element, ElementEditorContext context)
        {
            return base.OnBuildEditor(element, context);
        }
        protected override void OnDisplaying(ChatElement element, ElementDisplayContext context)
        {
            if (_services.Authorizer.Authorize(Orchard.Security.StandardPermissions.AccessAdminPanel))
                element.IsAdmin = true;

            element.LanguageResources = Controllers.ChatController.BuildClientResources();
            element.Version = Constants.JabbRVersion;
            context.ElementShape.Chat = element;
            base.OnDisplaying(element, context);
        }
    }
}
