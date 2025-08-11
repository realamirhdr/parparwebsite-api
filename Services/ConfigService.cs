using Mapster;
using Microsoft.EntityFrameworkCore;
using ParParWebsite.Api.DTOs;
using ParParWebsite.Api.Infrastructure;
using ParParWebsite.Api.Models;
using ParParWebsite.Api.Services.Interfaces;

namespace ParParWebsite.Api.Services;

public class ConfigService(AppDbContext context) : IConfigService
{
    public async Task Create(ConfigDTO configDto, CancellationToken cancellationToken)
    {
        var config = new Config()
        {
            ConfigName = configDto.ConfigName,
            ConfigValue = configDto.ConfigValue,
            CreatedAt = DateTime.Now
        };

        context.Config.Add(config);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ConfigDTO>> Get(CancellationToken cancellationToken)
    {
        var result = await context.Config
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        return result.Adapt<List<ConfigDTO>>();
    }

    public async Task Delete(int configId, CancellationToken cancellationToken)
    {
        var existing = await context.Config
            .FindAsync([configId], cancellationToken);

        if (existing != null)
        {
            context.Config.Remove(existing);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}