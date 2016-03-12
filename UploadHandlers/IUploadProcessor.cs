using Orchard;
using System.IO;
using System.Threading.Tasks;

namespace JabbR.UploadHandlers
{
    public interface IUploadProcessor : IDependency
    {
        Task<UploadResult> HandleUpload(string fileName, string contentType, Stream stream, long contentLength);
    }
}