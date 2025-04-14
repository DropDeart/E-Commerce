using Ljuss.Core.Domian;
using Ljuss.Applications.DTOs;
using Ljuss.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;

namespace LjussAPI.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public UserRepository(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _appDbContext.Users.ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(email))
                {
                    throw new ArgumentException("Email cannot be null or empty", nameof(email));
                }

                var user = await _appDbContext.Users.FirstOrDefaultAsync(p => p.Email == email);
                return user; 
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while getting user by email.");
                throw new Exception("Something went wrong!");
            }
        }

        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null.");
                }

                var existingUser = await _appDbContext.Users.AnyAsync(u => u.Email == user.Email);
                if (existingUser)
                {
                    throw new InvalidOperationException("A user with this email already exists.");
                }

                await _appDbContext.Users.AddAsync(user);
                await _appDbContext.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while adding a new user.");
                throw new Exception("Something went wrong while creating user.");
            }
        }

        public async Task<User> GetUserByIdAsync(Guid UserId)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(UserId.ToString()))
                { 
                    throw new ArgumentNullException(nameof(UserId));
                }

                var user = await _appDbContext.Users.FirstOrDefaultAsync(p => p.Id == UserId);

                return user;
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw new Exception("User can not exist");
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _appDbContext.Users.Where(p=>p.Id == user.Id).FirstOrDefaultAsync();
                if(existingUser == null)
                {
                    throw new InvalidOperationException($"User with ID {user.Id} not found.");
                }
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.IsVerified = user.IsVerified;
                existingUser.ProfilePictureUrl = user.ProfilePictureUrl;
                existingUser.PasswordHash = user.PasswordHash;

                _appDbContext.Users.Update(existingUser);
                var result = await _appDbContext.SaveChangesAsync();

                if (result == 0)
                {
                    throw new Exception("User update failed. No changes were made.");
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
                throw new Exception("User did not update");
            }
        }
    }
}
