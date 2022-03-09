using GeoLocator.Shared.Interfaces;

namespace GeoLocator.Core.Interfaces;
public interface IRepository<T> where T : IAggregateRoot
{
    Task<T> AddAsync(T entity);
}
