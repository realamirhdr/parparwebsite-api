using ParParWebsite.Api.Models.Enums;
using ParParWebsite.Api.Services.Interfaces;
using System.Threading;
using Microsoft.Net.Http.Headers;

namespace ParParWebsite.Api.Services
{
    public class FileService(IWebHostEnvironment webHostEnvironment) : IFileService
    {
        private readonly string _uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");

        public void DeleteFilesOfCollection(string collectionSlug, CancellationToken cancellationToken)
        {
            var directoryPath = Path.Combine(_uploadsPath, collectionSlug);

            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Directory path must be provided", nameof(directoryPath));

            if (!Directory.Exists(directoryPath))
                return; // nothing to do

            // This will remove all files and subdirectories
            Directory.Delete(directoryPath, recursive: true);
        }


        /// <summary>
        /// This method saves a single file to wwwroot path
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileType"></param>
        /// <param name="collectionSlug"></param>
        /// <param name="collectionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// The saved file's url
        /// </returns>
        private async Task<string> SaveFileAsync(IFormFile file, FileType fileType, string collectionSlug, int collectionId, CancellationToken cancellationToken)
        {
            // 1) Create a folder under wwwroot/uploads named after the slug of the portfolio
            var uploadDirectory = Path.Combine(_uploadsPath, collectionSlug);
            if (!Directory.Exists(uploadDirectory))
                Directory.CreateDirectory(uploadDirectory);

            // 2) Give each file a unique name => collectionId_type_index
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{collectionId}_{fileType.ToString().ToLower()}{extension}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            // 3) Save to disk
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            // 4) Return a URL relative to wwwroot:
            //    "/uploads/{collectionSlug}/{fileName}"
            var relativePath = Path.Combine("uploads", collectionSlug, fileName)
                .Replace("\\", "/");

            return "/" + relativePath;
        }
        
        /// <summary>
        /// This method saves a single file to wwwroot path
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// The saved file's url
        /// </returns>
        public async Task<string> SaveFileAsync(Stream stream, ContentDispositionHeaderValue contentDisposition, FileType fileType, string collectionSlug, int collectionId, CancellationToken cancellationToken, int? imageCount = null)
        {
            // 1) Create a folder under wwwroot/uploads named after the slug of the portfolio
            var uploadDirectory = Path.Combine(_uploadsPath, collectionSlug);
            if (!Directory.Exists(uploadDirectory))
                Directory.CreateDirectory(uploadDirectory);

            string imageCountStr = "";
            if (imageCount is not null)
            {
                imageCountStr = $"_{imageCount}";
            }

            // 2) Give each file a unique name => collectionId_type_index
            var extension = Path.GetExtension(HeaderUtilities.RemoveQuotes(contentDisposition.FileName).Value);
            var fileName = $"{collectionId}_{fileType.ToString().ToLower()}{imageCountStr}{extension}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            // 3) Save to disk
            await using var destinationStream = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(destinationStream, cancellationToken);

            // 4) Return a URL relative to wwwroot:
            //    "/uploads/{collectionSlug}/{fileName}"
            var relativePath = Path.Combine("uploads", collectionSlug, fileName)
                .Replace("\\", "/");

            return "/" + relativePath;

        }

        /// <summary>
        /// This method saves multiple files to wwwroot path
        /// </summary>
        /// <param name="files"></param>
        /// <param name="fileType"></param>
        /// <param name="collectionId"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="collectionSlug"></param>
        /// <returns>
        /// The saved files' url
        /// </returns>
        private async Task<List<string>> SaveMultipleFilesAsync(List<IFormFile> files, FileType fileType, string collectionSlug, int collectionId, CancellationToken cancellationToken)
        {
            // 1) Create a folder under wwwroot/uploads named after the slug of the portfolio
            var uploadDirectory = Path.Combine(_uploadsPath, collectionSlug);
            if (!Directory.Exists(uploadDirectory))
                Directory.CreateDirectory(uploadDirectory);

            var result = new List<string>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];

                // 2) Give each file a unique name => collectionId_type_index
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{collectionId}_{fileType.ToString().ToLower()}_{i}{extension}";
                var filePath = Path.Combine(uploadDirectory, fileName);

                // 3) Save to disk
                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream, cancellationToken);

                // 4) Return a URL relative to wwwroot:
                //    "/uploads/{collectionSlug}/{fileName}"
                var relativePath = Path.Combine("uploads", collectionSlug, fileName)
                    .Replace("\\", "/");

                result.Add("/" + relativePath);
            }

            return result;
        }


       
    }
}
