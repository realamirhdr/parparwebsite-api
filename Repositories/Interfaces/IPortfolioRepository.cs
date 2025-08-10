using ParParWebsite.Api.Models;

namespace ParParWebsite.Api.Repositories.Interfaces
{

    public interface IPortfolioRepository
    {
        Task<Portfolio> Create(Portfolio portfolio, CancellationToken cancellationToken);
        Task<Portfolio?> Update(Portfolio portfolio, CancellationToken cancellationToken);
        Task<bool> Delete(int collectionId, CancellationToken cancellationToken);
        Task<Portfolio?> GetById(int collectionId, CancellationToken cancellationToken);
        Task<List<Portfolio>> GetAll(CancellationToken cancellationToken);
    }


}
