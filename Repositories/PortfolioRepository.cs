using System.Reflection.Metadata.Ecma335;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ParParWebsite.Api.Infrastructure;
using ParParWebsite.Api.Models;
using ParParWebsite.Api.Repositories.Interfaces;

namespace ParParWebsite.Api.Repositories
{
    public class PortfolioRepository(AppDbContext context) : IPortfolioRepository
    {
        public async Task<Portfolio> Create(Portfolio portfolio, CancellationToken cancellationToken)
        {
            context.Portfolios.Add(portfolio);
            await context.SaveChangesAsync(cancellationToken);
            return portfolio;
        }

        public async Task<Portfolio?> Update(Portfolio portfolio, CancellationToken cancellationToken)
        {
            var existing = await context.Portfolios
                .FindAsync([portfolio.Id], cancellationToken);

            if (existing != null)
            {
                existing.Title = portfolio.Title;
                existing.Caption = portfolio.Caption;
                existing.ThumbnailUrl = portfolio.ThumbnailUrl;
                existing.Slug = portfolio.Slug;
                existing.Images = portfolio.Images;

                await context.SaveChangesAsync(cancellationToken);
            }

            return existing;
        }

        public async Task<bool> Delete(int collectionId, CancellationToken cancellationToken)
        {
            var existing = await context.Portfolios
                .FindAsync([collectionId], cancellationToken);

            if (existing != null)
            {
                context.Portfolios.Remove(existing);
                await context.SaveChangesAsync(cancellationToken);
                return true;
            }

            return false;
        }

        public async Task<Portfolio?> GetById(int collectionId, CancellationToken cancellationToken)
        {
            return await context.Portfolios
                .AsNoTracking()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(x => x.Id == collectionId, cancellationToken);
        }

        public async Task<List<Portfolio>> GetAll(CancellationToken cancellationToken)
        {
            return await context.Portfolios
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }

}
