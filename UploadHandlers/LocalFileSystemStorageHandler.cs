using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using JabbR.Services;

namespace JabbR.UploadHandlers
{
    public class LocalFileSystemStorageHandler : IUploadHandler
    {
        private readonly IApplicationSettings _settings;

        [ImportingConstructor]
        public LocalFileSystemStorageHandler(IApplicationSettings settings)
        {
            _settings = settings;
        }


        public bool IsValid(string fileName, string contentType)
        {
            //ApplicationSettings settings = _settings;

            // Blob storage can handle any content
            return (!String.IsNullOrEmpty(_settings.LocalFileSystemStoragePath) &&
                    !String.IsNullOrEmpty(_settings.LocalFileSystemStorageUriPrefix));
        }

        public async Task<UploadResult> UploadFile(string fileName, string contentType, Stream stream)
        {
            //ApplicationSettings settings = _settings;

            // Randomize the filename everytime so we don't overwrite files
            string randomFile = Path.GetFileNameWithoutExtension(fileName) +
                                "_" +
                                Guid.NewGuid().ToString().Substring(0, 4) + Path.GetExtension(fileName);


            if (!Directory.Exists(_settings.LocalFileSystemStoragePath))
            {
                Directory.CreateDirectory(_settings.LocalFileSystemStoragePath);
            }

            // REVIEW: Do we need to validate the settings here out of paranoia?
            
            string targetFile = Path.GetFullPath(Path.Combine(_settings.LocalFileSystemStoragePath, randomFile));

            using (FileStream destinationStream = File.Create(targetFile))
            {
                await stream.CopyToAsync(destinationStream);
            }


            var result = new UploadResult
            {
                Url = GetFullUrl(_settings.LocalFileSystemStorageUriPrefix, randomFile),
                Identifier = randomFile
            };

            return result;
        }

        private string GetFullUrl(string prefix, string fileName)
        {
            if (!prefix.EndsWith("/"))
            {
                prefix += "/";
            }

            var prefixUri = new Uri(prefix);
            return new Uri(prefixUri, fileName).ToString();
        }
    }
}