using Mapster;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using ParParWebsite.Api.DTOs;
using ParParWebsite.Api.Helper;
using ParParWebsite.Api.Infrastructure;
using ParParWebsite.Api.Models;
using ParParWebsite.Api.Models.Enums;
using ParParWebsite.Api.Services.Interfaces;

namespace ParParWebsite.Api.Services
{
    public class PortfolioService(AppDbContext context, IFileService fileService) : IPortfolioService
    {
        public async Task Create(Stream body, string boundary, CancellationToken cancellationToken)
        {
            var reader = new MultipartReader(boundary, body);

            var portfolio = new Portfolio();
            var isCollectionCreated = false;
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
                        context.Portfolios.Add(portfolio);
                        await context.SaveChangesAsync(cancellationToken);
                        portfolio.Images = new List<PortfolioImage>();
                        isCollectionCreated = true;
                    }

                    if (fieldName == "Thumbnail")
                    {
                        var thumbnailUrl = await fileService.SaveFileAsync(
                            section.Body,
                            contentDisposition,
                            FileType.Thumbnail,
                            portfolio.Slug,
                            portfolio.Id,
                            cancellationToken);

                        portfolio.ThumbnailUrl = thumbnailUrl;
                    }

                    if (fieldName == "Images")
                    {
                        var imageUrl = await fileService.SaveFileAsync(
                            section.Body,
                            contentDisposition,
                            FileType.Image,
                            portfolio.Slug,
                            portfolio.Id,
                            cancellationToken,
                            imageCount);

                        var collectionImage = new PortfolioImage()
                        {
                            ImageUrl = imageUrl,
                            Portfolio = portfolio,
                            CollectionId = portfolio.Id
                        };

                        portfolio.Images.Add(collectionImage);;

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

            await context.SaveChangesAsync(cancellationToken);
        }
        public async Task<List<PortfolioPreviewDTO>> Get(CancellationToken cancellationToken)
        {
            var portfolios = await context.Portfolios
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);

            return portfolios.Adapt<List<PortfolioPreviewDTO>>();
        }
        public async Task<PortfolioDTO> GetById(int collectionId, CancellationToken cancellationToken)
        {
            var portfolio = await context.Portfolios
                .AsNoTracking()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(x => x.Id == collectionId, cancellationToken);

            if (portfolio == null) { throw new Exception($"Portfolio {collectionId} not found."); }

            var dto = portfolio.Adapt<PortfolioDTO>();
            dto.Images = portfolio.Images.Adapt<List<PortfolioImageDTO>>();

            return dto;
        }
        public async Task Update(Stream body, string boundary, CancellationToken cancellationToken)
        {
            var reader = new MultipartReader(boundary, body);

            var portfolio = new Portfolio();
            portfolio.Images = new List<PortfolioImage>();
            var existing = new Portfolio();
            var imageCount = 1;
            var areFilesDeletedInitially = false;

            portfolio.LastUpdatedAt = DateTime.UtcNow;

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
                            portfolio.Slug,
                            portfolio.Id,
                            cancellationToken);

                        portfolio.ThumbnailUrl = thumbnailUrl;
                    }

                    if (fieldName == "Images")
                    {
                        var imageUrl = await fileService.SaveFileAsync(
                            section.Body,
                            contentDisposition,
                            FileType.Image,
                            portfolio.Slug,
                            portfolio.Id,
                            cancellationToken,
                            imageCount);

                        var collectionImage = new PortfolioImage()
                        {
                            ImageUrl = imageUrl,
                            Portfolio = portfolio,
                            CollectionId = portfolio.Id
                        };

                        portfolio.Images.Add(collectionImage); ;

                        imageCount++;
                    }
                }
                else
                {
                    using var r = new StreamReader(section.Body);
                    var value = await r.ReadToEndAsync(cancellationToken);

                    if (fieldName == "Id")
                    {
                        portfolio.Id = int.Parse(value);
                        existing = await context.Portfolios
                            .Include(p => p.Images)
                            .FirstOrDefaultAsync(x => x.Id == portfolio.Id, cancellationToken);

                        if (existing == null) { throw new Exception($"Portfolio {portfolio.Id} not found."); }
                    } 
                    else if (fieldName == "Title")
                    {
                        portfolio.Title = value;
                        portfolio.Slug = StringHelper.ToSlug(portfolio.Title);
                    }
                    else if (fieldName == "Caption")
                        portfolio.Caption = value;
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
        public async Task Delete(int collectionId, CancellationToken cancellationToken)
        {
            var existing = await context.Portfolios
                .FindAsync([collectionId], cancellationToken);

            if (existing == null) { throw new Exception($"Portfolio {collectionId} not found."); }

            fileService.DeleteFilesOfCollection(existing.Slug, cancellationToken);

            context.Portfolios.Remove(existing);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
