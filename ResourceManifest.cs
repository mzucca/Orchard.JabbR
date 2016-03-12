using Orchard.UI.Resources;

namespace JabbR.Infrastructure
{

    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            // TODO
            var manifest = builder.Add();
            manifest.DefineStyle("AdminEngagement").SetUrl("Engagement.css");
            manifest.DefineStyle("AdminHelptooltip").SetUrl("jquery.helptooltip.css");

            manifest.DefineStyle("ValidatorCss").SetUrl("cmxform.css");

            manifest.DefineScript("Validator").SetUrl("jquery.validate.min.js").SetDependencies("jQuery");
            manifest.DefineScript("ValidatorAdditional").SetUrl("additional-methods.min.js").SetDependencies("Validator");

        }
    }
}
