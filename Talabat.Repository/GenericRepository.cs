using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            //if (typeof(T) == typeof(Product))
            //    return (IEnumerable<T>)await _dbContext.Set<Product>().Include(P => P.Brand).Include(C => C.Category).ToListAsync();

            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            //if (typeof(T) == typeof(Product))
            //    return await _dbContext.Set<Product>().Where(P => P.Id == id).Include(P => P.Brand).Include(C => C.Category).FirstOrDefaultAsync() as T;

            return await _dbContext.Set<T>().FindAsync(id);
        }
        
        public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).ToListAsync();
        }

        public async Task<T?> GetWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }

        private IQueryable<T> ApplySpecifications(ISpecification<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
    }
}
