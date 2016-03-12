using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JabbR.Services;
using System.Reflection;
using JabbR.Infrastructure;

namespace JabbR.UploadHandlers
{
    public class UploadProcessor : IUploadProcessor
    {
        private readonly IList<IUploadHandler> _fileUploadHandlers;
        private readonly IApplicationSettings _settings;

        public UploadProcessor(IApplicationSettings settings)
        {
            _fileUploadHandlers = GetUploadHandlers(settings);
            _settings = settings;
        }

        public async Task<UploadResult> HandleUpload(string fileName, string contentType, Stream stream, long contentLength)
        {

            if (contentLength > _settings.MaxFileUploadBytes)
            {
                return new UploadResult { UploadTooLarge = true, MaxUploadSize = _settings.MaxFileUploadBytes };
            }

            string fileNameSlug = fileName.ToFileNameSlug();

            IUploadHandler handler = _fileUploadHandlers.FirstOrDefault(c => c.IsValid(fileNameSlug, contentType));

            if (handler == null)
            {
                return null;
            }

            return await handler.UploadFile(fileNameSlug, contentType, stream);
        }

        private static IList<IUploadHandler> GetUploadHandlers(IApplicationSettings settings)
        {
            // Use MEF to locate the content providers in this assembly
            var compositionContainer = new CompositionContainer(new AssemblyCatalog(typeof(UploadProcessor).Assembly));
            compositionContainer.ComposeExportedValue<IApplicationSettings>(settings);
            return compositionContainer.GetExportedValues<IUploadHandler>().ToList();
        }
    }
}