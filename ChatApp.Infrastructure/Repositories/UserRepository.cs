using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repositories
{
    /// <summary>
    /// Provides data access operations for users.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Where(x => !x.IsDeleted && x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Where(x => !x.IsDeleted && x.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetAllExceptAsync(Guid userId)
        {
            return await _context.Users
                .Where(x => !x.IsDeleted && x.Id != userId)
                .ToListAsync();
        }

      
        public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> userIds)
        {
            return await _context.Users
                .Where(x => !x.IsDeleted && userIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
