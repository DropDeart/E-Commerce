using Ljuss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Repositories
{
    public interface IShippingAddressRepository
    {
        IQueryable<ShippingAddress> GetAll(string? text = null, bool tracking = true);
        IQueryable<ShippingAddress> GetWhere(Expression<Func<ShippingAddress, bool>> predicate, bool tracking = true);
        Task<ShippingAddress?> GetSingleAsync(Expression<Func<ShippingAddress, bool>> predicate, bool tracking = true);
        Task<ShippingAddress?> GetByIdAsync(Guid id, bool tracking = true);
        Task<bool> AddAsync(ShippingAddress model);
        Task<bool> AddRangeAsync(List<ShippingAddress> models);
        bool Remove(ShippingAddress model);
        Task<bool> RemoveAsync(Guid id);
        bool RemoveRange(List<ShippingAddress> models);
        bool Update(ShippingAddress model);
        Task<bool> SaveAsync();
    }
}
