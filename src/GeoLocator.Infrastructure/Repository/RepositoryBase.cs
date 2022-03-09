using GeoLocator.Core.Interfaces;
using GeoLocator.Infrastructure.Data;
using GeoLocator.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeoLocator.Infrastructure.Repository;
public class RepositoryBase<T> : IRepository<T> where T : class, IAggregateRoot
{
    private readonly GeoLocatorDbContext _dbContext;

    public RepositoryBase(GeoLocatorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }
}
