using System.Collections;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly StoreContext _context;
    private Hashtable _repositories;
    public UnitOfWork(StoreContext context)
    {
        _context = context;
    }

    public async Task<int> Complete()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        _repositories ??= new Hashtable();

        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType
                .MakeGenericType(typeof(TEntity)), _context);

            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<TEntity>)_repositories[type];
    }


    //A better solution using dictionary
    // public IGenericRepository<T> Repository<T>() where T : class
    // {
    //     if (repositories.Keys.Contains(typeof(T)) == true)
    //     {
    //         return repositories[typeof(T)] as IRepository<T>;
    //     }
    //     IRepository<T> repo = new GenericRepository<T>(entities);
    //     repositories.Add(typeof(T), repo); return repo;
    // }
}
