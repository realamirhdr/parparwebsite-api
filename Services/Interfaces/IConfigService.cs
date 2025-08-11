using ParParWebsite.Api.DTOs;

namespace ParParWebsite.Api.Services.Interfaces;

public interface IConfigService
{
    Task Create(ConfigDTO configDto, CancellationToken cancellationToken);
    Task<List<ConfigDTO>> Get(CancellationToken cancellationToken);
    Task Delete(int collectionId, CancellationToken cancellationToken);
}