using Ljuss.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Repositories
{
    public interface IUserRepository
    {
        public Task<User>GetUserByEmailAsync(string email);
        public Task<User> GetUserByIdAsync(Guid UserId);
        public Task<User> AddUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task<IEnumerable<User>> GetAllAsync();
        public Task<bool> CheckUserPasswordAsync(Guid userId, string currentPassword);
    }
}
