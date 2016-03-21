using Orchard.UI.Resources;

namespace JabbR.Infrastructure
{

    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            // TODO
            var manifest = builder.Add();
            manifest.DefineStyle("bootstrap").SetUrl("bootstrap.min.css", "bootstrap.css");
            manifest.DefineStyle("font-awesome").SetUrl("font-awesome.min.css", "font-awesome.css");
            manifest.DefineStyle("emoji20").SetUrl("emoji20.css");
            manifest.DefineStyle("defaultChatBbc").SetUrl("themes/default/Chat.bbcnews.css");
            manifest.DefineStyle("defaultChatDictionary").SetUrl("themes/default/Chat.dictionary.css");
            manifest.DefineStyle("defaultChatGit").SetUrl("themes/default/Chat.githubissues.css");
            manifest.DefineStyle("defaultChatNuGet").SetUrl("themes/default/Chat.nuget.css");
            manifest.DefineStyle("defaultChat").SetUrl("themes/default/Chat.css").SetDependencies("defaultChatBbc", "defaultChatDictionary", "defaultChatGit", "defaultChatNuGet");
            manifest.DefineStyle("github").SetUrl("Highlight/github.css");
            manifest.DefineStyle("keyTips").SetUrl("KeyTips.css");

            manifest.DefineScript("jquery2").SetUrl("jquery-2.0.3.min.js", "jquery-2.0.3.js");
            manifest.DefineScript("jqueryMigrate2").SetUrl("jquery-migrate-1.2.1.min.js", "jquery-migrate-1.2.1.js");
            manifest.DefineScript("json2").SetUrl("json2.min.js", "json2.js");
            manifest.DefineScript("bootstrap").SetUrl("bootstrap.min.js", "bootstrap.js");
            manifest.DefineScript("jqueryColor").SetUrl("jquery.color-2.1.2.min.js", "jquery.color-2.1.2.js");
            manifest.DefineScript("jqueryDragsort").SetUrl("jquery.dragsort-0.5.1.min.js", "jquery.dragsort-0.5.1.js");
            manifest.DefineScript("jqueryPulse").SetUrl("jquery.pulse.min.js", "jquery.pulse.js");
            manifest.DefineScript("jqueryKeyTips").SetUrl("jquery.KeyTips.min.js", "jquery.KeyTips.js");
            manifest.DefineScript("jquerySignalR").SetUrl("jquery.signalR-2.2.0.min.js", "jquery.signalR-2.2.0.js");

            manifest.DefineScript("hubs").SetUrl("~/signalr/hubs").SetDependencies(
                "jquery2",
                "jqueryMigrate2",
                "json2",
                "bootstrap",
                "jqueryColor",
                "jqueryDragsort",
                "jqueryPulse",
                "jqueryKeyTips",
                "jquerySignalR"
                );


            manifest.DefineScript("jQueryTmpl").SetUrl("jQuery.tmpl.min.js", "jQuery.tmpl.js");
            manifest.DefineScript("jqueryCookie2").SetUrl("jquery.cookie.js");
            manifest.DefineScript("jqueryAutotabcomplete").SetUrl("jquery.autotabcomplete.js");
            manifest.DefineScript("jqueryTimeago").SetUrl("jquery.timeago.0.10.js");
            manifest.DefineScript("jqueryCaptureDocumentWrite").SetUrl("jquery.captureDocumentWrite.min.js", "jquery.captureDocumentWrite.js");
            manifest.DefineScript("jquerySortElements").SetUrl("jquery.sortElements.js");
            manifest.DefineScript("baLinkify").SetUrl("ba-linkify.min.js");
            manifest.DefineScript("quicksilver").SetUrl("quicksilver.js");
            manifest.DefineScript("marked").SetUrl("marked.js");
            manifest.DefineScript("jqueryHistory").SetUrl("jquery.history.js");
            manifest.DefineScript("moment").SetUrl("moment.min.js");
            manifest.DefineScript("highlight").SetUrl("highlight.pack.js");
            manifest.DefineScript("livestamp").SetUrl("livestamp.min.js", "livestamp.js");
            manifest.DefineScript("Chat.emoji").SetUrl("Chat.emoji.js");
            manifest.DefineScript("Chat.utility").SetUrl("Chat.utility.js");
            manifest.DefineScript("Chat.toast").SetUrl("Chat.toast.js");
            manifest.DefineScript("Chat.ui.room").SetUrl("Chat.ui.room.js");
            manifest.DefineScript("Chat.ui").SetUrl("Chat.ui.js");
            manifest.DefineScript("Chat.ui.fileUpload").SetUrl("Chat.ui.fileUpload.js");
            manifest.DefineScript("Chat.documentOnWrite").SetUrl("Chat.documentOnWrite.js");
            manifest.DefineScript("Chat.twitter").SetUrl("Chat.twitter.js");
            manifest.DefineScript("Chat.githubissues").SetUrl("Chat.githubissues.js");
            manifest.DefineScript("Chat").SetUrl("Chat.js").SetDependencies(
                "jQueryTmpl",
                "jqueryCookie2",
                "jqueryAutotabcomplete",
                "jqueryTimeago",
                "jqueryCaptureDocumentWrite",
                "jquerySortElements",
                "baLinkify",
                "quicksilver",
                "marked",
                "jqueryHistory",
                "moment",
                "highlight",
                "livestamp",
                "Chat.emoji",
                "Chat.utility",
                "Chat.toast",
                "Chat.ui.room",
                "Chat.ui",
                "Chat.ui.fileUpload",
                "Chat.documentOnWrite",
                "Chat.twitter",
                "Chat.githubissues"
                );
            manifest.DefineScript("loader").SetUrl("loader.js");
        }
    }
}
