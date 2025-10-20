using Ljuss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Attribute = Ljuss.Core.Entities.Attribute;

namespace Ljuss.Core.Repositories
{
    public interface IAttributeRepository
    {
        IQueryable<Attribute> GetAll(string? text = null, bool tracking = true);
        IQueryable<Attribute> GetWhere(Expression<Func<Attribute, bool>> predicate, bool tracking = true);
        Task<Attribute?> GetSingleAsync(Expression<Func<Attribute, bool>> predicate, bool tracking = true);
        Task<Attribute?> GetByIdAsync(Guid id, bool tracking = true);
        Task<bool> AddAsync(Attribute model);
        Task<bool> AddRangeAsync(List<Attribute> models);
        bool Remove(Attribute model);
        Task<bool> RemoveAsync(Guid id);
        bool RemoveRange(List<Attribute> models);
        bool Update(Attribute model);
        Task<bool> SaveAsync();
    }
}

