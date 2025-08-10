using ParParWebsite.Api.DTOs;
using ParParWebsite.Api.Models;

namespace ParParWebsite.Api.Services.Interfaces
{
    public interface IPortfolioService
    {
        Task Create(Stream body, string boundary, CancellationToken cancellationToken);
        Task<List<PortfolioPreviewDTO>> Get(CancellationToken cancellationToken);
        Task<PortfolioDTO> GetById(int collectionId, CancellationToken cancellationToken);
        Task Update(Stream body, string boundary, CancellationToken cancellationToken);
        Task Delete(int collectionId, CancellationToken cancellationToken);
    }
}
