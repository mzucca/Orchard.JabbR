using Orchard;
using System.Threading.Tasks;

namespace JabbR.ContentProviders.Core
{
    public interface IResourceProcessor : ISingletonDependency
    {
        Task<ContentProviderResult> ExtractResource(string url);
    }
}
