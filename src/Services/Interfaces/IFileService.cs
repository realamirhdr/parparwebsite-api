using Microsoft.Net.Http.Headers;
using ParParWebsite.Api.Models.Enums;

namespace ParParWebsite.Api.Services.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Deletes all files related to one portfolio.
        /// </summary>
        /// <param name="postSlug"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentException"></exception>
        void DeleteFilesOfCollection(string postSlug, CancellationToken cancellationToken);

        Task<string> SaveFileAsync(Stream stream, ContentDispositionHeaderValue contentDisposition, FileType fileType,
            string collectionSlug, int collectionId, CancellationToken cancellationToken, int? imageCount = null);
    }
}
