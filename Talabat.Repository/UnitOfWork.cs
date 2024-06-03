using System.Collections;
using System.Collections.Concurrent;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;

        private readonly ConcurrentDictionary<string, object> _repositories;

        public UnitOfWork(StoreContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new ConcurrentDictionary<string, object>();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var key = typeof(TEntity).Name;
            //if (!_repositories.ContainsKey(key))
            //{
            //    var repo = new GenericRepository<TEntity>(_dbContext);
            //    _repositories.Add(key, repo);
            //}

            //return _repositories[key] as IGenericRepository<TEntity>;
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(key, _ => new GenericRepository<TEntity>(_dbContext));

        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }

    }
}
