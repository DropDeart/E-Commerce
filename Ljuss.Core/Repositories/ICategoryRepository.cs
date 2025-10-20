using Ljuss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetAll(string? text = null, bool tracking = true);
        IQueryable<Category> GetWhere(Expression<Func<Category, bool>> predicate, bool tracking = true);
        Task<Category?> GetSingleAsync(Expression<Func<Category, bool>> predicate, bool tracking = true);
        Task<Category?> GetByIdAsync(Guid id, bool tracking = true);
        Task<bool> AddAsync(Category model);
        Task<bool> AddRangeAsync(List<Category> models);
        bool Remove(Category model);
        Task<bool> RemoveAsync(Guid id);
        bool RemoveRange(List<Category> models);
        bool Update(Category model);
        Task<bool> SaveAsync();

    }
}
