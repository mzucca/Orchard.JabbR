using Orchard.Core.Navigation;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace JabbR {

    [OrchardFeature("JabbR")]
    public class ChatAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
                .AddImageSet("settings")
                    .Add(T("JabbR"), "1", elements => elements.Action("Settings", "ChatAdmin", new { area = "JabbR" }).Permission(StandardPermissions.SiteOwner)
                        .Add(T("Settings"), "1.1", item => item.Action("Settings", "ChatAdmin", new { area = "JabbR" }).Permission(StandardPermissions.SiteOwner).LocalNav())
                        .Add(T("Q&A"), "1.2", item => item.Action("Index", "QuestionAnswer", new { area = "JabbR" }).Permission(StandardPermissions.SiteOwner).LocalNav())
                        .Add(T("Rooms"), "1.3", item => item.Action("Index", "RoomsAdmin", new { area = "JabbR" }).Permission(StandardPermissions.SiteOwner).LocalNav())
                    );
        }
    }
}