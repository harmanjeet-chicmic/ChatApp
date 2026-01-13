using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId);

        // ✅ ADD THIS
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> userIds);
        Task AddAsync(User user);
        
        Task<IEnumerable<User>> GetAllExceptAsync(Guid currentUserId);
        Task UpdateAsync(User user);
    }
}


// using ChatApp.Domain.Entities;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace ChatApp.Application.Interfaces.Repositories
// {
//     /// <summary>
//     /// Defines data access operations for users.
//     /// </summary>
//     public interface IUserRepository
//     {
//         
//         

//         Task<IEnumerable<User>> GetAllExceptAsync(Guid userId);

//         Task AddAsync(User user);
//         Task UpdateAsync(User user);
//     }
// }
