using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using ParParWebsite.Api.DTOs;
using ParParWebsite.Api.Helper;
using ParParWebsite.Api.Models;
using ParParWebsite.Api.Models.Enums;
using ParParWebsite.Api.Repositories;
using ParParWebsite.Api.Repositories.Interfaces;
using ParParWebsite.Api.Services.Interfaces;

namespace ParParWebsite.Api.Services
{
    public class PortfolioService(IPortfolioRepository repository, IFileService fileService) : IPortfolioService
    {

        public async Task Create(Stream body, string boundary, CancellationToken cancellationToken)
        {
            var reader = new MultipartReader(boundary, body);

            var portfolio = new Portfolio();
            var isCollectionCreated = false;
            var created = new Portfolio();
            var imageCount = 1;

            portfolio.CreatedAt = DateTime.UtcNow;
            portfolio.LastUpdatedAt = DateTime.UtcNow;

            while (await reader.ReadNextSectionAsync(cancellationToken) is { } section)
            {
                var contentDisposition = ContentDispositionHeaderValue.Parse(section.ContentDisposition);

                var fieldName = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;

                var isFile = !string.IsNullOrEmpty(contentDisposition.FileName.Value);

                if (isFile)
                {
                    // not file props are done
                    if (!isCollectionCreated)
                    {
                        created = await repository.Create(portfolio, cancellationToken);
                        created.Images = new List<PortfolioImage>();
                        isCollectionCreated = true;
                    }

                    if (fieldName == "Thumbnail")
                    {
                        var thumbnailUrl = await fileService.SaveFileAsync(
                            section.Body,
                            contentDisposition,
                            FileType.Thumbnail,
                            created.Slug,
                            created.Id,
                            cancellationToken);

                        created.ThumbnailUrl = thumbnailUrl;
                    }

                    if (fieldName == "Images")
                    {
                        var imageUrl = await fileService.SaveFileAsync(
                            section.Body,
                            contentDisposition,
                            FileType.Image,
                            created.Slug,
                            created.Id,
                            cancellationToken,
                            imageCount);

                        var collectionImage = new PortfolioImage()
                        {
                            ImageUrl = imageUrl,
                            Portfolio = created,
                            CollectionId = created.Id
                        };

                        created.Images.Add(collectionImage);;

                        imageCount++;
                    }
                }
                else
                {
                    using var r = new StreamReader(section.Body);
                    var value = await r.ReadToEndAsync(cancellationToken);

                    if (fieldName == "Title")
                    {
                        portfolio.Title = value;
                        portfolio.Slug = StringHelper.ToSlug(portfolio.Title);
                    }
                    else if (fieldName == "Caption")
                        portfolio.Caption = value;
                }
            }

            await repository.Update(created, cancellationToken);
        }
        public async Task<List<PortfolioPreviewDTO>> Get(CancellationToken cancellationToken)
        {
            var portfolios = await repository.GetAll(cancellationToken);

            return portfolios.Adapt<List<PortfolioPreviewDTO>>();
        }
        public async Task<PortfolioDTO> GetById(int collectionId, CancellationToken cancellationToken)
        {
            var collection = await repository.GetById(collectionId, cancellationToken);

            if (collection == null) { throw new Exception($"Portfolio {collectionId} not found."); }

            var dto = collection.Adapt<PortfolioDTO>();
            dto.Images = collection.Images.Adapt<List<PortfolioImageDTO>>();

            return dto;
        }
        public async Task Update(Stream body, string boundary, CancellationToken cancellationToken)
        {
            var reader = new MultipartReader(boundary, body);

            var collection = new Portfolio();
            collection.Images = new List<PortfolioImage>();
            var existing = new Portfolio();
            var imageCount = 1;
            var areFilesDeletedInitially = false;

            collection.LastUpdatedAt = DateTime.UtcNow;

            while (await reader.ReadNextSectionAsync(cancellationToken) is { } section)
            {
                var contentDisposition = ContentDispositionHeaderValue.Parse(section.ContentDisposition);

                var fieldName = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;

                var isFile = !string.IsNullOrEmpty(contentDisposition.FileName.Value);

                if (isFile)
                {
                    // not file props are done
                    if (!areFilesDeletedInitially)
                    {
                        var slug = existing.Slug;

                        fileService.DeleteFilesOfCollection(slug, cancellationToken);

                        areFilesDeletedInitially = true;
                    }
                   
                    if (fieldName == "Thumbnail")
                    {
                        var thumbnailUrl = await fileService.SaveFileAsync(
                            section.Body,
                            contentDisposition,
                            FileType.Thumbnail,
                            collection.Slug,
                            collection.Id,
                            cancellationToken);

                        collection.ThumbnailUrl = thumbnailUrl;
                    }

                    if (fieldName == "Images")
                    {
                        var imageUrl = await fileService.SaveFileAsync(
                            section.Body,
                            contentDisposition,
                            FileType.Image,
                            collection.Slug,
                            collection.Id,
                            cancellationToken,
                            imageCount);

                        var collectionImage = new PortfolioImage()
                        {
                            ImageUrl = imageUrl,
                            Portfolio = collection,
                            CollectionId = collection.Id
                        };

                        collection.Images.Add(collectionImage); ;

                        imageCount++;
                    }
                }
                else
                {
                    using var r = new StreamReader(section.Body);
                    var value = await r.ReadToEndAsync(cancellationToken);

                    if (fieldName == "Id")
                    {
                        collection.Id = int.Parse(value);
                        existing = await repository.GetById(collection.Id, cancellationToken);

                        if (existing == null) { throw new Exception($"Portfolio {collection.Id} not found."); }
                    } 
                    else if (fieldName == "Title")
                    {
                        collection.Title = value;
                        collection.Slug = StringHelper.ToSlug(collection.Title);
                    }
                    else if (fieldName == "Caption")
                        collection.Caption = value;
                }
            }

            await repository.Update(collection, cancellationToken);
        }
        public async Task Delete(int collectionId, CancellationToken cancellationToken)
        {
            var existing = await repository.GetById(collectionId, cancellationToken);

            if (existing == null) { throw new Exception($"Portfolio {collectionId} not found."); }

            fileService.DeleteFilesOfCollection(existing.Slug, cancellationToken);

            await repository.Delete(collectionId, cancellationToken);
        }
    }
}
